# smugmug-image-of-the-day
Pulls a random image from a set of albums or folders on SmugMug and posts the image url to an IFTTT maker channel


This project is meant to be run by some sort of scheduled job performing a post against the image controller, but has a simple web form to manually initiate the action.

The action will perform a POST to the given target IFTTT Maker url with a json payload of:

` { "value1": [imageUrl] }