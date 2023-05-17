using System;
using System.Runtime.InteropServices;

namespace AnodyneSharp
{
    public static class Achievements
    {
        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamAPI_Init();

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_Shutdown();

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_RunCallbacks();

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SteamInternal_CreateInterface(
            [MarshalAs(UnmanagedType.LPStr)]
                string pchVersion
        );

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SteamAPI_GetHSteamUser();

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SteamAPI_GetHSteamPipe();

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SteamAPI_ISteamClient_GetISteamUserStats(
            IntPtr steamClient,
            int steamUser,
            int steamPipe,
            [MarshalAs(UnmanagedType.LPStr)]
                string pchVersion
        );

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamAPI_ISteamUserStats_RequestCurrentStats(
            IntPtr instance
        );

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamAPI_ISteamUserStats_StoreStats(
            IntPtr instance
        );

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamAPI_ISteamUserStats_SetAchievement(
            IntPtr instance,
            [MarshalAs(UnmanagedType.LPStr)]
                string name
        );

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamAPI_ISteamUserStats_SetStatInt32(
            IntPtr instance,
            [MarshalAs(UnmanagedType.LPStr)]
                string name,
            int stat
        );

        public static bool WasInit = false;
        private static IntPtr steamUserStats;

        public static void Init()
        {
            try
            {
                WasInit = SteamAPI_Init();
            }
            catch
            {
                WasInit = false;
            }
            if (WasInit)
            {
                IntPtr steamClient = SteamInternal_CreateInterface(
                    "SteamClient020"
                );
                int steamUser = SteamAPI_GetHSteamUser();
                int steamPipe = SteamAPI_GetHSteamPipe();
                steamUserStats = SteamAPI_ISteamClient_GetISteamUserStats(
                    steamClient,
                    steamUser,
                    steamPipe,
                    "STEAMUSERSTATS_INTERFACE_VERSION012"
                );
                SteamAPI_ISteamUserStats_RequestCurrentStats(steamUserStats);
            }
        }

        public static void Update()
        {
            if (WasInit)
            {
                SteamAPI_RunCallbacks();
            }
        }

        public static void Quit()
        {
            if (WasInit)
            {
                SteamAPI_Shutdown();
                WasInit = false;
            }
        }

        public static void Unlock(string achievement)
        {
            if (WasInit)
            {
                SteamAPI_ISteamUserStats_SetAchievement(steamUserStats, achievement);
                SteamAPI_ISteamUserStats_StoreStats(steamUserStats);
            }
        }

        public static void SetStat(string name, int stat)
        {
            if (WasInit)
            {
                SteamAPI_ISteamUserStats_SetStatInt32(steamUserStats, name, stat);
                SteamAPI_ISteamUserStats_StoreStats(steamUserStats);
            }
        }
    }
}
