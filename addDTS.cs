using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace miniAP
{
    class Program
    {
  
        static void Main(string[] args)
        {            
            string[] Months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            //setting up the directory where the files we wish to convert live
            DirectoryInfo autoDir = new DirectoryInfo(@"C:\Users\blandreneau\Desktop\1612000466\");
            FileInfo[] filesInAuto = null;
            filesInAuto = autoDir.GetFiles();

            //converting file extension to MP4 -- this is fine for an MTS, not sure about other file types
            foreach (FileInfo file in filesInAuto)
            {
                string fileName = file.ToString(); //converting fileinfo to string
                string extension = file.Extension.ToString(); //getting the extension fo the file
                string pathToVideo = autoDir + fileName;


                //renaming the file
                System.IO.File.Move(pathToVideo, autoDir + fileName.Replace(extension, ".MP4")); //renaming the file
            }

            foreach (FileInfo file in filesInAuto)
            {
                //creating vars for processing date/time stamp
                string dateTimeOriginal = null; //stores value from exif data
                string dateStamp = null;  //stores value for writing date/timestamp to vide
                string timeStamp = null;
                string time, date; //vars for date and time -- they are separate entities to break down into smaller parts
                string day, stringMonth, year = null; //vars for date, time is simpler so no need to break it into hours,min,seconds
                int intMonth = 0;
                string fileName = file.ToString();
                string pathToVideo = autoDir + fileName;

                dateTimeOriginal = getDateTimeOriginal(pathToVideo);
                string[] tokensFromDateTimeOriginal = dateTimeOriginal.Split(' '); //split the exif data into two parts, date and time

                date = tokensFromDateTimeOriginal[0];
                time = tokensFromDateTimeOriginal[1];

                string[] tokensFromDate = date.Split(':');
                string[] tokensFromTime = time.Split('-');

                time = tokensFromTime[0]; //we don't need the part after the "-" in dateTimeOriginal
                time = time.Replace(":", "\\:") + "\\:00";
                time = "\\'" + time + "\\'";

                Console.WriteLine(time);

                //Converting exif mon/day/yr info to how it should look on autoProc'd video
                year = tokensFromDate[0];
                stringMonth = tokensFromDate[1]; //stringMonth starts as string and comes from 2nd element of tokensFromDate
                intMonth = Int32.Parse(stringMonth); //convert stringMonth to intMonth to get proper month name jan/feb/nov/etc
                stringMonth = Months[intMonth - 1];
                day = tokensFromDate[2];

                //these are the formatted strings for ffmpeg to write to the video
                //to ensure that they are centered, we must treat them as separate entities
                dateStamp = stringMonth + " " + day + " " + year; //date formatted to HUB standards (Nov 13 2016)
                timeStamp = time; //time formatted to ffmpeg argument form
		    
                addDateAndTimeStamp(autoDir.ToString(), fileName, dateStamp, timeStamp);    
            }

            Console.ReadLine();


        }
        

        public static string getDateTimeOriginal(string pathToVideo)
        {
            

            string outputFromExif = null;
            string dateTimeOriginal = null;
            string searchFor = "\"DateTimeOriginal\":";
            

            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "exiftool.exe";
                //startInfo.Arguments = "-j C:\\Users\\blandreneau\\Desktop\\00003.MTS";
                startInfo.Arguments = "-j " + pathToVideo;

                process.StartInfo = startInfo;
                process.Start();
                outputFromExif = process.StandardOutput.ReadToEnd().Replace("[", "").Replace("]", "");
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //locate "Make:" in the exif data, then take everything from "Make:" to the end of the exif data.
            //after this, we simply truncate everything at/after the next comma.
            if (outputFromExif.Contains(searchFor))
            {
                //MessageBox.Show("found make!");
                dateTimeOriginal = outputFromExif.Substring(outputFromExif.IndexOf(searchFor) + searchFor.Length);
                // MessageBox.Show("stringFromExif: " + cameraModel);
                int delAfterChar = dateTimeOriginal.IndexOf(",");
                if (delAfterChar > 0)
                {
                    dateTimeOriginal = dateTimeOriginal.Substring(0, delAfterChar).Replace("\"", "").Trim();
                }
                else
                {
                    dateTimeOriginal = "NA";
                }
            }
            else
            {
                dateTimeOriginal = "NA";
                //MessageBox.Show(cameraModel);
            }


            return dateTimeOriginal;

        }

        public static void addDateAndTimeStamp(string pathToVideos, string videoName, string dateStamp, string timeStamp)
        {
            //setting up strings for ffmpeg out/err
            string stdOutput = null;
            string stdError = null;

            string pathToIndividualVideo = pathToVideos + videoName; //gets us the path to the videoName
            string fontfile = "\'/Windows/Fonts/Nimbus.otf\'"; //Directory to the fontfile
            string fontSize = "40";

            try
            {

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = false; //we don't need the shell
                startInfo.RedirectStandardOutput = true; //we are redirecting stdOut to this program
                startInfo.RedirectStandardError = true; //we are redirecting stdErr to this program
                startInfo.CreateNoWindow = true; //no need to create an extra window
                startInfo.FileName = "ffmpeg.exe"; //we are running ffmpeg, make sure ffmpeg is in your %PATH
                startInfo.Arguments = " -i " + pathToIndividualVideo + " -c:v h264_qsv -b:v 4000k -an -vf \"[in]drawtext=fontsize=" + fontSize + ":fontcolor=White:fontfile=" + fontfile + ":timecode=" + timeStamp + ": rate=30: fontsize=40:fontcolor='white':x=w-tw-155: y=h-th-100, drawtext=fontsize=" + fontSize + ":fontcolor=White:fontfile=" + fontfile + ": text='" + dateStamp +"': x=w-tw-150: y=h-th-150[out]\" " + pathToVideos + "/DTS" + videoName;
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
            Console.WriteLine("STDOUT: \n" + stdOutput);
            Console.WriteLine("STDERR: \n" + stdError);
            
        }
    }
}
