using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeFormatter
{
    public static class Formatter
    {
        static Formatter()
        { }
        public static void Do(string source,string destination,string file)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string extension = Path.GetExtension(file);
            string newFile = file.Replace(source, destination);
            string path = Path.GetDirectoryName(newFile);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //open
            FileStream streamOld = new FileStream(file, FileMode.Open, FileAccess.Read);
            StreamReader readerOld=new StreamReader(streamOld);
            FileStream streamNew = new FileStream(newFile, FileMode.Create, FileAccess.Write);
            StreamWriter writerNew = new StreamWriter(streamNew);
            //
            writerNew.WriteLine("//Modified by CodeFormatter@Zhang Yongquan");
            //
            if (extension == ".h")
            {
                writerNew.WriteLine("#ifndef _" + name + "_H");
                writerNew.WriteLine("#define _" + name + "_H");
                writerNew.WriteLine("#include \"lib_saic.h\"");
                HeaderFile(readerOld, writerNew);
                writerNew.WriteLine("#endif");
            }
            else if (extension == ".c")
            {
                writerNew.WriteLine("#include  \"" + name + ".h\"");
                CodeFile(readerOld, writerNew);
            }
            //while ((line = readerOld.ReadLine()) != null)
            //{
            
            //    writerNew.WriteLine(line);
            //}
          //close
            writerNew.Close();
            readerOld.Close();
            streamOld.Close();
            streamNew.Close();
        }
        private static void HeaderFile(StreamReader reader, StreamWriter writer)
        {
            string line = "";
            bool flagStruct = false, flagUnused = false;
            
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains("extern uint8 ") || line.Contains("extern sint8 "))
                {
                    writer.WriteLine("#define START_SECTION_OEM_volatile_FastRam_8bit");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                }
                else if (line.Contains("extern uint16 ")|| line.Contains("extern sint16 "))
                {
                    writer.WriteLine("#define START_SECTION_OEM_volatile_FastRam_16bit");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                }
                else if (line.Contains("extern uint32 ") || line.Contains("extern sint32 "))
                {
                    writer.WriteLine("#define START_SECTION_OEM_volatile_FastRam_32bit");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                }
                else if (line.Contains("struct "))
                {
                    writer.WriteLine("#define START_SECTION_OEM_Caldata_32bit");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                    if (line.Contains("{")) flagStruct = true;
                }
                else if (line.Contains("#if defined(COMPILE_UNUSED_CODE)"))
                {
                    flagUnused = true;
                    continue;
                }
                else if (line.Contains("#endif") && flagUnused)
                {
                    flagUnused = false;
                    continue;
                }
                else if (line.Contains("void "))
                {
                    writer.WriteLine("#define START_SECTION_Task_OEM_r10ms");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                }

                writer.WriteLine(line);
                if (line.Contains("extern uint8 ") || line.Contains("extern sint8 "))
                {
                    writer.WriteLine("#define STOP_SECTION_OEM_volatile_FastRam_8bit");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                }
                else if (line.Contains("extern uint16 ") || line.Contains("extern sint16 "))
                {
                    writer.WriteLine("#define STOP_SECTION_OEM_volatile_FastRam_16bit");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                }
                else if (line.Contains("extern uint32 ") || line.Contains("extern sint32 "))
                {
                    writer.WriteLine("#define STOP_SECTION_OEM_volatile_FastRam_32bit");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                }
                else if (line.Contains("struct ") && !flagStruct)
                {
                    writer.WriteLine("#define STOP_SECTION_OEM_Caldata_32bit");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                }
                else if (line.Contains("};") && flagStruct)
                {
                    writer.WriteLine("#define STOP_SECTION_OEM_Caldata_32bit");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                    flagStruct = false;
                }
                else if (line.Contains("void "))
                {
                    writer.WriteLine("#define STOP_SECTION_Task_OEM_r10ms");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                }
            }
            
        }
        private static void CodeFile(StreamReader reader, StreamWriter writer)
        {
            string line = "";
            bool flagStruct = false, flagUnused = false, flagFunction = false, flagAddress = false;
            int flag = 0;
            while ((line = reader.ReadLine()) != null)
            {
                if (line == "#pragma section neardata \"IRAM\"" || line == "#pragma section farrom \"EROM\"" || line == "#pragma section code \"ECODE\"")
                {
                    continue;
                }
                else if (line.Contains("BEGIN: ASCET REGION \"Virtual Address Table\""))
                {
                    flagAddress = true;
                    continue;
                }
                else if (line.Contains( "END: ASCET REGION \"Virtual Address Table\""))
                {
                    flagAddress = false;
                    continue;
                }
                else if (flagAddress)
                {
                    continue;
                }
                if (line == " * BEGIN: DEFINITION OF MESSAGES")
                { 
                    flag = 1;//message
                }
                else if (line == " * END: DEFINITION OF MESSAGES")
                {
                    flag = 0;
                }
                else if (line == " * BEGIN: FUNCTIONS OF COMPONENT")
                {
                    flag = 2;//function
                }
                else if (line == " * END: FUNCTIONS OF COMPONENT")
                {
                    flag = 0;
                }
                switch (flag)
                {
                    case 1:
                        if (line.Contains("uint8 ") || line.Contains("sint8 "))
                        {
                            writer.WriteLine("#define START_SECTION_OEM_volatile_FastRam_8bit");
                            writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                        }
                        else if (line.Contains("uint16 ") || line.Contains("sint16 "))
                        {
                            writer.WriteLine("#define START_SECTION_OEM_volatile_FastRam_16bit");
                            writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                        }
                        else if (line.Contains("uint32 ") || line.Contains("sint32 "))
                        {
                            writer.WriteLine("#define START_SECTION_OEM_volatile_FastRam_32bit");
                            writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                        }
                        if (line.Contains("= true;"))
                        {
                            line.Replace("= true;", "= false;");
                        }
                        else if (line.Contains("= false;") || line.Contains("= 0;"))
                        {
                            //OK
                        }
                        else if(line.Contains(" = "))
                        {
                            int index = line.IndexOf('=');
                            line = line.Remove(index);
                            line += "= 0;";
                        }
                        break;
                    case 2: 

                        break;
                    default: 
                        break;
                }

                
                if (line.Contains("struct ") && !flagStruct)
                {
                    writer.WriteLine("#define START_SECTION_OEM_Caldata_32bit");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                    if (line.Contains("{")) flagStruct = true;
                }
                else if (line.Contains("#if defined(COMPILE_UNUSED_CODE)"))
                {
                    flagUnused = true;
                    continue;
                }
                else if (line.Contains("#endif") && flagUnused)
                {
                    flagUnused = false;
                    continue;
                }
                else if (line.Contains("void "))
                {
                    writer.WriteLine("#define START_SECTION_Task_OEM_r10ms");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                    flagFunction = true;
                    if (line.Contains("{")) flagStruct = true;
                }
                writer.WriteLine(line);
                switch (flag)
                {
                    case 1:
                        if (line.Contains("uint8 ") || line.Contains("sint8 "))
                        {
                            writer.WriteLine("#define STOP_SECTION_OEM_volatile_FastRam_8bit");
                            writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                        }
                        else if (line.Contains("uint16 ") || line.Contains("sint16 "))
                        {
                            writer.WriteLine("#define STOP_SECTION_OEM_volatile_FastRam_16bit");
                            writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                        }
                        else if (line.Contains("uint32 ") || line.Contains("sint32 "))
                        {
                            writer.WriteLine("#define STOP_SECTION_OEM_volatile_FastRam_32bit");
                            writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                        }
                        break;
                    default: 
                        break;
                }
                
                if (line.Contains("struct ") && !flagStruct)
                {
                    writer.WriteLine("#define STOP_SECTION_OEM_Caldata_32bit");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                }
                else if (line.Contains("}") && flagStruct) 
                {
                    writer.WriteLine("#define STOP_SECTION_OEM_Caldata_32bit");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                    flagStruct = false;
                }
                else if (line.IndexOf("}")==0 && flagFunction)
                {
                    writer.WriteLine("#define STOP_SECTION_Task_OEM_r10ms");
                    writer.WriteLine("#include \"swsh_uaes2oem.h\"");
                    flagFunction = false;
                }
            }
        }
    }
}
