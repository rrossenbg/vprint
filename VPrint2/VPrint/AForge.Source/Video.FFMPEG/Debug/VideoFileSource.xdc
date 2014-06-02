<?xml version="1.0"?>
<doc>
<members>
<member name="T:AForge.Video.FFMPEG.VideoFileSource" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="19">
<summary>
Video source for video files.
</summary>

<remarks><para>The video source provides access to video files using <a href="http://www.ffmpeg.org/">FFmpeg</a> library.</para>

<para><note>The class provides video only. Sound is not supported.</note></para>

<para><note>The class ignores presentation time of video frames while retrieving them from
video file. Instead it provides video frames according to the FPS rate of the video file
or the configured <see cref="P:AForge.Video.FFMPEG.VideoFileSource.FrameInterval"/>.</note></para>

<para><note>Make sure you have <b>FFmpeg</b> binaries (DLLs) in the output folder of your application in order
to use this class successfully. <b>FFmpeg</b> binaries can be found in Externals folder provided with AForge.NET
framework's distribution.</note></para>

<para>Sample usage:</para>
<code>
// create video source
VideoFileSource videoSource = new VideoFileSource( fileName );
// set NewFrame event handler
videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
// start the video source
videoSource.Start( );
// ...

// New frame event handler, which is invoked on each new available video frame
private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
{
    // get new frame
    Bitmap bitmap = eventArgs.Frame;
    // process the frame
}
</code>
</remarks>

</member>
<member name="E:AForge.Video.FFMPEG.VideoFileSource.NewFrame" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="59">
<summary>
New frame event.
</summary>

<remarks><para>Notifies clients about new available frame from video source.</para>

<para><note>Since video source may have multiple clients, each client is responsible for
making a copy (cloning) of the passed video frame, because the video source disposes its
own original copy after notifying of clients.</note></para>
</remarks>

</member>
<member name="E:AForge.Video.FFMPEG.VideoFileSource.VideoSourceError" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="72">
<summary>
Video source error event.
</summary>

<remarks>This event is used to notify clients about any type of errors occurred in
video source object, for example internal exceptions.</remarks>

</member>
<member name="E:AForge.Video.FFMPEG.VideoFileSource.PlayingFinished" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="81">
<summary>
Video playing finished event.
</summary>

<remarks><para>This event is used to notify clients that the video playing has finished.</para>
</remarks>

</member>
<member name="P:AForge.Video.FFMPEG.VideoFileSource.Source" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="90">
<summary>
Video source.
</summary>

<remarks><para>Video file name to play.</para></remarks>

</member>
<member name="P:AForge.Video.FFMPEG.VideoFileSource.FramesReceived" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="108">
<summary>
Received frames count.
</summary>

<remarks>Number of frames the video source provided from the moment of the last
access to the property.
</remarks>

</member>
<member name="P:AForge.Video.FFMPEG.VideoFileSource.BytesReceived" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="126">
<summary>
Received bytes count.
</summary>

<remarks>Number of bytes the video source provided from the moment of the last
access to the property.
</remarks>

</member>
<member name="P:AForge.Video.FFMPEG.VideoFileSource.IsRunning" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="142">
<summary>
State of the video source.
</summary>

<remarks>Current state of video source object - running or not.</remarks>

</member>
<member name="P:AForge.Video.FFMPEG.VideoFileSource.FrameInterval" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="164">
<summary>
Frame interval.
</summary>

<remarks><para>The property sets the interval in milliseconds between frames. If the property is
set to 100, then the desired frame rate will be 10 frames per second.</para>

<para><note>Setting this property to 0 leads to no delay between video frames - frames
are read as fast as possible.</note></para>

<para><note>Setting this property has effect only when <see cref="P:AForge.Video.FFMPEG.VideoFileSource.FrameIntervalFromSource"/>
is set to <see langword="false"/>.</note></para>

<para>Default value is set to <b>0</b>.</para>
</remarks>

</member>
<member name="P:AForge.Video.FFMPEG.VideoFileSource.FrameIntervalFromSource" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="192">
<summary>
Get frame interval from source or use manually specified.
</summary>

<remarks><para>The property specifies which frame rate to use for video playing.
If the property is set to <see langword="true"/>, then video is played
with original frame rate, which is set in source video file. If the property is
set to <see langword="false"/>, then custom frame rate is used, which is
calculated based on the manually specified <see cref="P:AForge.Video.FFMPEG.VideoFileSource.FrameInterval">frame interval</see>.</para>

<para>Default value is set to <see langword="true"/>.</para>
</remarks>

</member>
<member name="M:AForge.Video.FFMPEG.VideoFileSource.#ctor(System.String)" decl="true" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="219">
<summary>
Initializes a new instance of the <see cref="T:AForge.Video.FFMPEG.VideoFileSource"/> class.
</summary>

</member>
<member name="M:AForge.Video.FFMPEG.VideoFileSource.Start" decl="true" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="225">
<summary>
Start video source.
</summary>

<remarks>Starts video source and return execution to caller. Video source
object creates background thread and notifies about new frames with the
help of <see cref="E:AForge.Video.FFMPEG.VideoFileSource.NewFrame"/> event.</remarks>

<exception cref="T:System.ArgumentException">Video source is not specified.</exception>

</member>
<member name="M:AForge.Video.FFMPEG.VideoFileSource.SignalToStop" decl="true" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="237">
<summary>
Signal video source to stop its work.
</summary>

<remarks>Signals video source to stop its background thread, stop to
provide new frames and free resources.</remarks>

</member>
<member name="M:AForge.Video.FFMPEG.VideoFileSource.WaitForStop" decl="true" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="246">
<summary>
Wait for video source has stopped.
</summary>

<remarks>Waits for source stopping after it was signalled to stop using
<see cref="M:AForge.Video.FFMPEG.VideoFileSource.SignalToStop"/> method.</remarks>

</member>
<member name="M:AForge.Video.FFMPEG.VideoFileSource.Stop" decl="true" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilesource.h" line="255">
<summary>
Stop video source.
</summary>

<remarks><para>Stops video source aborting its thread.</para>

<para><note>Since the method aborts background thread, its usage is highly not preferred
and should be done only if there are no other options. The correct way of stopping camera
is <see cref="M:AForge.Video.FFMPEG.VideoFileSource.SignalToStop">signaling it stop</see> and then
<see cref="M:AForge.Video.FFMPEG.VideoFileSource.WaitForStop">waiting</see> for background thread's completion.</note></para>
</remarks>

</member>
<member name="T:AForge.Video.FFMPEG.VideoFileReader" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="20">
<summary>
Class for reading video files utilizing FFmpeg library.
</summary>

<remarks><para>The class allows to read video files using <a href="http://www.ffmpeg.org/">FFmpeg</a> library.</para>

<para><note>Make sure you have <b>FFmpeg</b> binaries (DLLs) in the output folder of your application in order
to use this class successfully. <b>FFmpeg</b> binaries can be found in Externals folder provided with AForge.NET
framework's distribution.</note></para>

<para>Sample usage:</para>
<code>
// create instance of video reader
VideoFileReader reader = new VideoFileReader( );
// open video file
reader.Open( "test.avi" );
// check some of its attributes
Console.WriteLine( "width:  " + reader.Width );
Console.WriteLine( "height: " + reader.Height );
Console.WriteLine( "fps:    " + reader.FrameRate );
Console.WriteLine( "codec:  " + reader.CodecName );
// read 100 video frames out of it
for ( int i = 0; i &lt; 100; i++ )
{
    Bitmap videoFrame = reader.ReadVideoFrame( );
    // process the frame somehow
    // ...

    // dispose the frame when it is no longer required
    videoFrame.Dispose( );
}
reader.Close( );
</code>
</remarks>

</member>
<member name="P:AForge.Video.FFMPEG.VideoFileReader.Width" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="59">
<summary>
Frame width of the opened video file.
</summary>

<exception cref="T:System.IO.IOException">Thrown if no video file was open.</exception>

</member>
<member name="P:AForge.Video.FFMPEG.VideoFileReader.Height" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="74">
<summary>
Frame height of the opened video file.
</summary>

<exception cref="T:System.IO.IOException">Thrown if no video file was open.</exception>

</member>
<member name="P:AForge.Video.FFMPEG.VideoFileReader.FrameRate" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="89">
<summary>
Frame rate of the opened video file.
</summary>

<exception cref="T:System.IO.IOException">Thrown if no video file was open.</exception>

</member>
<member name="P:AForge.Video.FFMPEG.VideoFileReader.FrameCount" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="104">
<summary>
Number of video frames in the opened video file.
</summary>

<remarks><para><note><b>Warning</b>: some video file formats may report different value
from the actual number of video frames in the file (subject to fix/investigate).</note></para>
</remarks>

<exception cref="T:System.IO.IOException">Thrown if no video file was open.</exception>

</member>
<member name="P:AForge.Video.FFMPEG.VideoFileReader.CodecName" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="123">
<summary>
Name of codec used for encoding the opened video file.
</summary>

<exception cref="T:System.IO.IOException">Thrown if no video file was open.</exception>

</member>
<member name="P:AForge.Video.FFMPEG.VideoFileReader.IsOpen" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="138">
<summary>
The property specifies if a video file is opened or not by this instance of the class.
</summary>
</member>
<member name="M:AForge.Video.FFMPEG.VideoFileReader.Finalize" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="151">
<summary>
Object's finalizer.
</summary>

</member>
<member name="M:AForge.Video.FFMPEG.VideoFileReader.#ctor" decl="true" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="162">
<summary>
Initializes a new instance of the <see cref="T:AForge.Video.FFMPEG.VideoFileReader"/> class.
</summary>

</member>
<member name="M:AForge.Video.FFMPEG.VideoFileReader.Dispose" decl="false" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="168">
<summary>
Disposes the object and frees its resources.
</summary>

</member>
<member name="M:AForge.Video.FFMPEG.VideoFileReader.Open(System.String)" decl="true" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="178">
<summary>
Open video file with the specified name.
</summary>

<param name="fileName">Video file name to open.</param>

<exception cref="T:System.IO.IOException">Cannot open video file with the specified name.</exception>
<exception cref="T:AForge.Video.VideoException">A error occurred while opening the video file. See exception message.</exception>

</member>
<member name="M:AForge.Video.FFMPEG.VideoFileReader.ReadVideoFrame" decl="true" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="189">
<summary>
Read next video frame of the currently opened video file.
</summary>

<returns>Returns next video frame of the opened file or <see langword="null"/> if end of
file was reached. The returned video frame has 24 bpp color format.</returns>

<exception cref="T:System.IO.IOException">Thrown if no video file was open.</exception>
<exception cref="T:AForge.Video.VideoException">A error occurred while reading next video frame. See exception message.</exception>

</member>
<member name="M:AForge.Video.FFMPEG.VideoFileReader.Close" decl="true" source="c:\projects\vprint\vprint\aforge.source\video.ffmpeg\videofilereader.h" line="201">
<summary>
Close currently opened video file if any.
</summary>

</member>
</members>
</doc>
