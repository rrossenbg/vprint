using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace VPrint.Common
{
    // * A
    // Allocate struct in unmanaged memory
    /*
    static void Main(string[] args)
    {
        using (var chunk = new StructAllocator())
        {
            Console.WriteLine(">> Simple data test");
            SimpleDataTest(chunk);

            Console.WriteLine();

            Console.WriteLine(">> Complex data test");
            ComplexDataTest(chunk);
        }

        Console.ReadLine();
    }

    private static void SimpleDataTest(StructAllocator chunk)
    {
        IntPtr ptr = chunk.Allocate<System.Int32>();

        Console.WriteLine(StructAllocator.ConvertPointerToStruct<Int32>(ptr));
        Debug.Assert(StructAllocator.ConvertPointerToStruct<Int32>(ptr) == 0, "Data not initialized properly");
        Debug.Assert(chunk.AllocatedMemory == sizeof(Int32), "Data not allocated properly");

        int data = ChunkAllocator.ConvertPointerToStruct<Int32>(ptr);
        data = 10;
        StructAllocator.StoreStructure(ptr, data);

        Console.WriteLine(StructAllocator.ConvertPointerToStruct<Int32>(ptr));
        Debug.Assert(StructAllocator.ConvertPointerToStruct<Int32>(ptr) == 10, "Data not set properly");

        Console.WriteLine("All tests passed");
    }

    private static void ComplexDataTest(StructAllocator chunk)
    {
        IntPtr ptr = chunk.Allocate<Person>();

        Console.WriteLine(StructAllocator.ConvertPointerToStruct<Person>(ptr));
        Debug.Assert(StructAllocator.ConvertPointerToStruct<Person>(ptr).Age == 0, "Data age not initialized properly");
        Debug.Assert(StructAllocator.ConvertPointerToStruct<Person>(ptr).Name == null, "Data name not initialized properly");
        Debug.Assert(chunk.AllocatedMemory == Marshal.SizeOf(typeof(Person)) + sizeof(Int32), "Data not allocated properly");

        Person data = StructAllocator.ConvertPointerToStruct<Person>(ptr);
        data.Name = "Bob";
        data.Age = 20;
        StructAllocator.StoreStructure(ptr, data);

        Console.WriteLine(StructAllocator.ConvertPointerToStruct<Person>(ptr));
        Debug.Assert(StructAllocator.ConvertPointerToStruct<Person>(ptr).Age == 20, "Data age not set properly");
        Debug.Assert(StructAllocator.ConvertPointerToStruct<Person>(ptr).Name == "Bob", "Data name not set properly");

        Console.WriteLine("All tests passed");
    }

    struct Person
    {
        public string Name;
        public int Age;

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return "Age is " + Age;
            return Name + " is " + Age + " years old";
        }
    }
    */
    public sealed class StructAllocator : IDisposable
    {
        IntPtr m_chunkStart;
        int m_offset;   //offset from already allocated memory
        readonly int m_size;

        public StructAllocator(int memorySize = 1024)
        {
            if (memorySize < 1)
                throw new ArgumentOutOfRangeException("memorySize must be positive");

            m_size = memorySize;
            m_chunkStart = Marshal.AllocHGlobal(memorySize);
        }

        ~StructAllocator()
        {
            Dispose();
        }

        public IntPtr Allocate<T>() where T : struct
        {
            int reqBytes = Marshal.SizeOf(typeof(T));//not highly performant
            return Allocate<T>(reqBytes);
        }

        public IntPtr Allocate<T>(int reqBytes) where T : struct
        {
            if (m_chunkStart == IntPtr.Zero)
                throw new ObjectDisposedException("ChunkAllocator");
            if (m_offset + reqBytes > m_size)
                throw new OutOfMemoryException("Too many bytes allocated: " + reqBytes + " needed, but only " + (m_size - m_offset) + " bytes available");

            T created = default(T);
            Marshal.StructureToPtr(created, m_chunkStart + m_offset, false);
            m_offset += reqBytes;

            return m_chunkStart + (m_offset - reqBytes);
        }

        public void Dispose()
        {
            if (m_chunkStart != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_chunkStart);
                m_offset = 0;
                m_chunkStart = IntPtr.Zero;
            }
        }

        public void ReleaseAllMemory()
        {
            m_offset = 0;
        }

        public int AllocatedMemory
        {
            get { return m_offset; }
        }

        public int AvailableMemory
        {
            get { return m_size - m_offset; }
        }

        public int TotalMemory
        {
            get { return m_size; }
        }

        public static T ConvertPointerToStruct<T>(IntPtr ptr) where T : struct
        {
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        public static void StoreStructure<T>(IntPtr ptr, T data) where T : struct
        {
            Marshal.StructureToPtr(data, ptr, false);
        }
    }

    // * C
    // Allocate C# classes in unmanaged memory
    //*Usage*/
    /*
        using (var a = new ObjectHandle<A>())
        {
            a.Value.M1 = 0;
            a.Value.M2 = "Rosen";
            a.Value.M3 = 0;
        }
    */
    public class ObjectHandle<T> : IDisposable where T : class
    {
        private bool m_freed;
        private IntPtr m_handle;
        private readonly T m_value;
        private readonly IntPtr m_tptr;

        public ObjectHandle()
            : this(typeof(T))
        {

        }

        public ObjectHandle(Type t)
        {
            m_tptr = t.TypeHandle.Value;
            int size = Marshal.ReadInt32(m_tptr, 4);//base instance size
            m_handle = Marshal.AllocHGlobal(size);
            byte[] zero = new byte[size];
            Marshal.Copy(zero, 0, m_handle, size);//zero memory

            IntPtr ptr = m_handle + 4;
            Marshal.WriteIntPtr(ptr, m_tptr);//write type ptr
            m_value = GetO(ptr);//convert to reference
        }

        public T Value
        {
            get
            {
                return m_value;
            }
        }

        public bool Valid
        {
            get
            {
                return Marshal.ReadIntPtr(m_handle, 4) == m_tptr;
            }
        }

        public void Dispose()
        {
            if (!m_freed)
            {
                Marshal.FreeHGlobal(m_handle);
                m_freed = true;
                GC.SuppressFinalize(this);
            }
        }

        ~ObjectHandle()
        {
            Dispose();
        }

        delegate T GetO_d(IntPtr ptr);
        static readonly GetO_d GetO;

        static ObjectHandle()
        {
            DynamicMethod m = new DynamicMethod("GetO", typeof(T), new[] { typeof(IntPtr) }, typeof(ObjectHandle<T>), true);
            var il = m.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ret);
            GetO = m.CreateDelegate(typeof(GetO_d)) as GetO_d;
        }
    }
}