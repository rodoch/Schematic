namespace Schematic.Core
{
    public class SchematicSettings
    {
        public string ApplicationName { get; set; } = "Schematic";

        public string ApplicationDescription { get; set; }

        public ApplicationIconSettings ApplicationIcon { get; set; }

        public string AssetDirectory { get; set; }

        public string AssetWebPath { get; set; }

        public CloudStorageSettings CloudStorage { get; set; }

        public string ContactEmail { get; set; }

        public DataStoreSettings DataStore { get; set; }

        public EmailSettings Email { get; set; }

        public double SetPasswordTimeLimitHours { get; set; } = 24;
    }
}