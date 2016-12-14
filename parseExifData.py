#!/usr/bin/env python

'''
This code will take an input video and use Phil Harvey's exifTool to check the exif data of said video.  
I suggest downloading exifTool and running it with some sample video to get a feel of what kind of exif data
is obtained.  Note that different recording devices will generate different exif data!
'''

from subprocess import PIPE, run
import sys
import json

#pass in path to file you wish to parse exif data from
pathToFile = sys.argv[1];
#print(pathToFile)


def getExifData(video):

    # run the command "exiftool.exe -j yourInputVideo"
    command = ["exiftool.exe", "-j", video]

    # we want the ouput of the command, So PIPE stdout/stderr
    result = run(command, stdout=PIPE, stderr=PIPE, universal_newlines=True)

    # for python to treat this as a json, remove the brackets from the start and finish
    # there may be a better way to do this, but this seems to work fine.
    outputString = str(result.stdout).strip().replace("[", "").replace("]", "")

    # test output of exiftool command
    # print(outputString)

    # load the string as a json object, then search for the make key
    jsonDict = json.loads(outputString)

    videoInfo = {}

    #building personalized dictionary for exif data (key/values that are important)
    if "FileName" in jsonDict:
        videoInfo["Filename"] = jsonDict["FileName"]
    else:
        videoInfo["Filname"] = "NULL"

    if "Make" in jsonDict:
        videoInfo["Make"] = jsonDict["Make"]
    else:
        videoInfo["Make"] = "NULL"

    if "Duration" in jsonDict:
        videoInfo["Duration"] = jsonDict["Duration"]
    else:
        videoInfo["Duration"] = "NULL"

    if "DateTimeOriginal" in jsonDict:
        videoInfo["DateTimeOriginal"] = jsonDict["DateTimeOriginal"]
    else:
        videoInfo["DateTimeOriginal"] = "DateTimeOriginal"

    return videoInfo

videoExifData = getExifData(sys.argv[1])

for key, value in videoExifData.items():
    print(key, ":", value)










