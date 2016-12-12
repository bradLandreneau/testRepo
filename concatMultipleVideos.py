#!/usr/bin/env python

import os
import glob
import re
import sys
from ffmpy import FFmpeg

print(sys.path)

#getting arguments from command line
pathToVideos = sys.argv[1]
convertFrom = sys.argv[2]
outputFileName = sys.argv[3]

########################################################################################################################
#use this to convert MP4 files to TS files (useful in concatenation of videos)
def convertVideostoTS(convertFrom):

    files = glob.glob('*' + convertFrom)

    for file in files:
        print(file)
        #creating output filename
        #newFileName = "TStest" + file

        #get rid of extension, it will eventually be replaced with .ts
        newFileName = re.sub(convertFrom, '', file)
        print(newFileName)

        #converting .mp4 files to .ts files (adding .ts extension as well)
        ff = FFmpeg(
            inputs = {file: None},
            outputs = {newFileName+".ts": '-c copy -bsf:v h264_mp4toannexb -f mpegts'}
        )
        ff.cmd
        ff.run()


def concatVideos(stringOfFiles, outputFileName):

    ff = FFmpeg(
        inputs={stringOfFiles: None},
        outputs={outputFileName: '-c copy'}
    )

    ff.cmd
    ff.run()

########################################################################################################################

os.chdir(pathToVideos)

convertVideostoTS(convertFrom)

# now, find all .ts files in the folder and concatenate them
files = glob.glob('*.ts')
stringOfFiles = '|'.join(files)
stringOfFiles = "concat:" + stringOfFiles

concatVideos(stringOfFiles, outputFileName)

print("done")










