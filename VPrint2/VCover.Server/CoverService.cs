using System;

namespace VCover.Server
{
    public class CoverService : ICoverService
    {
        public string[] ReadData(Guid id)
        {
            return new string[10];
        }

        public void SaveData(Guid id, string imagePath)
        {
            AppContext.Default.NewImage(imagePath);
        }
    }
}
