namespace Preferences
{
    public static class Settings
    {

        public static class Config
        {
            /*
             * 1. Rifle
             * 2. Machine Gun
             * 3. Laser
             * 4. Shotgun
             * 5. Bazooka
             */
            public const int weaponSlotsCount = 5;
        }
        public static class Tags
        {
            public const string Player = "Player";
            public const string Enemy = "Enemy";
            public const string Weapon = "Weapon";
            public const string Ricochetable = "Ricochetable";
        }

        public static class Layers
        {
            public const string Player = "Player";
        }

        public static class GameObjects
        {
            public const string GlobalController = "Global Controller";
        }

    }
}