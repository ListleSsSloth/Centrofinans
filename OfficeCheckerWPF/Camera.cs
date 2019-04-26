namespace OfficeCheckerWPF
{
    internal class Camera
    {
        public readonly string ConfigXmlMjpegurl;
        public readonly string ConfigXmlAviFileName;
        public readonly string ConfigXmlFeatureSet;
        public readonly string ConfigXmlRecentFileList;

        public readonly string ObjectsXmlSourceindex;
        public readonly string ObjectsXmlVideoSourceString;

        public Camera(string configXmlMjpegurl, string configXmlAviFileName, string configXmlFeatureSet, string configXmlRecentFileList, string objectsXmlSourceindex, string objectsXmlVideoSourceString)
        {
            ConfigXmlMjpegurl = configXmlMjpegurl;
            ConfigXmlAviFileName = configXmlAviFileName;
            ConfigXmlFeatureSet = configXmlFeatureSet;
            ConfigXmlRecentFileList = configXmlRecentFileList;

            ObjectsXmlSourceindex = objectsXmlSourceindex;
            ObjectsXmlVideoSourceString = objectsXmlVideoSourceString;
        }
    }
}