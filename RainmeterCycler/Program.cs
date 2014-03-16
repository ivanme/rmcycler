using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace RainmeterCycler
{
    class Program
    {
        const string 
            DEFAULT_INSTALL_FOLDER = @"C:\Program Files\Rainmeter\Rainmeter.exe",
            DEFAULT_LAYOUT_FOLDER = @"\Rainmeter\Layouts";
       
        const string    
            RAINMETER_CYCLER = "RMCycler",
            RM_PATH = "RMPath",
            LAYOUT_PATH = "LFPath",
            LIST = "List",
            MODE = "Mode",
            REMAINING = "Remaining";

        const string
            RANDOM = "Random",
            SHUFFLE = "Shuffle",
            INORDER = "InOrder";

        static void Main(string[] args)
        {
            RunRMCycler((args.Length==1) ? TrimQuotes(args[0]) : null);  
        }

        static void RunRMCycler(string configFilePath)
        {
            bool givenConfigFile = !string.IsNullOrEmpty(configFilePath);

            SectionData configData = null;
            if (givenConfigFile) GetDataFromFile(configFilePath, ref configData);

            string rainmeter_exe = GetRmLocation(configData);
            string arguments = "!LoadLayout " + "\"" + GetLayoutName(configData) + "\"";
            
            Run(rainmeter_exe, arguments);

            if (givenConfigFile)
            {
                SaveConfigData(configFilePath, ref configData);
            }
        }

        static void GetDataFromFile(string configFilePath, ref SectionData configData)
        {
            try
            {
                FileIniDataParser iniParser = new FileIniDataParser();
                IniData parsedData = iniParser.ReadFile(configFilePath);
                configData = parsedData.Sections.GetSectionData(RAINMETER_CYCLER);
            }
            catch (IniParser.Exceptions.ParsingException e)
            {
                configData = null;
            }
        }

        static void Run(string rainmeter_exe, string arguments)
        {
            try
            {
                System.Diagnostics.Process.Start(rainmeter_exe, arguments);
            }
            catch(System.ComponentModel.Win32Exception e)
            {
                //File not found
            }
        }

        static void SaveConfigData(string configFilePath, ref SectionData configData)
        {
            try
            {
                FileIniDataParser iniParser = new FileIniDataParser();
                IniData parsedData = iniParser.ReadFile(configFilePath);
                parsedData.Sections.SetSectionData(RAINMETER_CYCLER, configData);
                iniParser.WriteFile(configFilePath, parsedData);
            }
            catch (Exception e)
            {

            }
        }

        static string GetRmLocation(SectionData data)
        {
            string result = null;
            if(data != null && data.Keys.ContainsKey(RM_PATH))
            {
                result = TrimQuotes(data.Keys[RM_PATH]);
            }
            return (string.IsNullOrEmpty(result)) ? DEFAULT_INSTALL_FOLDER : result;   
        }
        
        static string GetLayoutName(SectionData data)
        {
            string[] layoutList = null;
            GetLayouts(data, ref layoutList);

            if (layoutList == null) return "";

            return SelectLayout(data, ref layoutList);
        }
                
        static void GetLayouts(SectionData data, ref string[] layoutNames)
        {
            string 
                INI_LayoutPath = null, 
                INI_Collection = null, 
                INI_Remaining = null,
                INI_Mode = null;

            if(data != null)
            {
                INI_LayoutPath = TrimQuotes(data.Keys[LAYOUT_PATH]);
                INI_Collection = data.Keys[LIST];
                INI_Remaining = data.Keys[REMAINING];
                INI_Mode = data.Keys[MODE];
            }

            if (string.IsNullOrEmpty(INI_Remaining) || string.Equals(INI_Mode,RANDOM))
            {
                if (string.IsNullOrEmpty(INI_Collection))
                {
                    GetAllLayouts(INI_LayoutPath, ref layoutNames);
                }
                else
                {
                    GetSpecifiedLayouts(INI_Collection, ref layoutNames);
                }
            }
            else if (string.Equals(INI_Mode, SHUFFLE) || string.Equals(INI_Mode, INORDER))
            {
                GetSpecifiedLayouts(INI_Remaining, ref layoutNames);
            }
            else
            {
                GetAllLayouts(INI_LayoutPath, ref layoutNames);
            }

        }

        static void GetAllLayouts(string layoutFolderPath, ref string[] layoutPool)
        {
            if (string.IsNullOrEmpty(layoutFolderPath))
            {
                layoutFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + DEFAULT_LAYOUT_FOLDER;
            }
            else
            {
                layoutFolderPath = TrimQuotes(layoutFolderPath);
            }

            DirectoryInfo layoutDir = new DirectoryInfo(layoutFolderPath);

            if (layoutDir.Exists)
            {
                List<string> alistofstring = new List<string>();
                DirectoryInfo[] anArrayOfDirectoryInfo = layoutDir.GetDirectories();

                foreach (DirectoryInfo aDirectoryInfo in anArrayOfDirectoryInfo)
                {
                    alistofstring.Add(aDirectoryInfo.ToString());
                }

                alistofstring.Remove("@Backup");
                layoutPool = alistofstring.ToArray();
            }

        }

        static void GetSpecifiedLayouts(string LayoutList, ref string[] layoutPool)
        {
            layoutPool = LayoutList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        static string SelectLayout(SectionData data, ref string[] layoutList)
        {
            int selectedLayoutIndex;

            string Selection_Mode = (data != null) ? data.Keys[MODE] : null;

            if (string.Equals(Selection_Mode, INORDER))
            {
                selectedLayoutIndex = 0;
            }
            else
            {
                Random rand1 = new Random();
                selectedLayoutIndex = rand1.Next(layoutList.Length);
            }

            string selectedLayoutName = layoutList[selectedLayoutIndex];

            if (string.Equals(Selection_Mode, SHUFFLE) || (string.Equals(Selection_Mode, INORDER)))
            {
                layoutList.SetValue(null, selectedLayoutIndex);
                layoutList = layoutList.Where(s => !string.IsNullOrEmpty(s)).ToArray();
                data.Keys[REMAINING] = string.Join(",", layoutList);
            }

            return selectedLayoutName;
        }

        static string TrimQuotes(string input)
        {
            return (string.IsNullOrEmpty(input)) ? "" : input.Trim(new char[] { '\"' });
        }
    }
}
