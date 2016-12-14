using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ffmpegBumper
{
    class Program
    {
        static void Main(string[] args)
        {
            //create strings for stdOut and stdErr
            string stdOutput = null;
            string stdError = null;

            //create strings to add to video clip
            string firstLine = "hello world!";
            string firstName = "brad";
            string lastName = "landreneau\\, ";
            string fontSize = "36";

            //directory you wish to work in
            string directory = "C:\\Users\\blandreneau\\Desktop\\bumperTest\\";
            Directory.SetCurrentDirectory(directory);
            //Console.WriteLine(Directory.GetCurrentDirectory());

            try
            {
                
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = false; //we don't need the shell
                startInfo.RedirectStandardOutput = true; //we are redirecting stdOut to this program
                startInfo.RedirectStandardError = true; //we are redirecting stdErr to this program
                startInfo.CreateNoWindow = true; //no need to create an extra window
                startInfo.FileName = "ffmpeg.exe"; //we are running ffmpeg, make sure ffmpeg is in your %PATH

                //this extremely long line will output three lines, centered in the screen. 
                //not quite sure how to do multiline, bash arguments that work well with c# syntax at this time.
                startInfo.Arguments = "-f lavfi -i color=c=black:s=1280x720:d=5: -vf \"[in]drawtext=fontsize=" + fontSize + ":fontcolor=White:fontfile='/Windows/Fonts/coolvetica.ttf':text='On Screen Text!':x=(main_w/2-text_w/2):y=(h)/2, drawtext=fontsize=" + fontSize + ":fontcolor=White:fontfile='/Windows/Fonts/coolvetica.ttf':text=" + firstLine + ":x=(main_w/2-text_w/2):y=((h)/2)+40, drawtext=fontsize=" + fontSize + ":fontcolor=White:fontfile='/Windows/Fonts/coolvetica.ttf':text=" + lastName + firstName + ":x=(main_w/2-text_w/2):y=((h)/2)+80[out]\" -y test_out.mp4";

                //Console.WriteLine(startInfo.Arguments.ToString()); //if you want to see how the string is formatted when sent to console

                process.StartInfo = startInfo;
                process.Start();
                stdError = process.StandardError.ReadToEnd();
                stdOutput = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine(stdOutput);
            Console.WriteLine(stdError);
            Console.ReadKey();
        }
    }
}
