#!/usr/bin/env python
# -*- coding: utf-8 -*-

import os

# Imports the Flask wrapper, abort function and request context
from flask import Flask, abort, request, render_template, session, redirect
import random, string, time
import logging
import requests

from werkzeug.contrib.cache import SimpleCache
from imgurpython import ImgurClient

#Create a little persistent cache object, just for example's sake
cache = SimpleCache(default_timeout=0)

#Import some helper functions
import settings
import base64

client_id = '6851efbe5a89371'
client_secret = '5695aadbbf64eb5e5767c9614654763c9e2e0dcd'

client = ImgurClient(client_id, client_secret)


# Creates an instance of the flask server using *this* module as its unique identifier
app = Flask(__name__, template_folder='views')


#This is a function decorator, it basically is a middleware that attaches the function hello to the flask gateway
@app.route("/")
def home():
	name1=cache.get('name1')
	finished1=cache.get('finished1')
	image1=cache.get('image1')

	name2=cache.get('name2')
	finished2=cache.get('finished2')
	image2=cache.get('image2')

	return render_template('index.html', name1=name1, image1=image1, finished1=finished1, name2=name2, image2=image2, finished2=finished2)

@app.route('/update/1', methods=['POST'])
def hello():
	name = request.form['name_field']
	finished = request.form['finished_field']
	image = request.form['image']

	cache.set('name1', name)
	cache.set('finished1', finished)
	cache.set('image1', image)

	return "Success"


@app.route('/update/2', methods=['POST'])
def hello2():
	name = request.form['name_field']
	finished = request.form['finished_field']
	image = request.form['image']

	cache.set('name2', name)
	cache.set('finished2', finished)
	cache.set('image2', image)

	return "Success"


@app.route('/poll')
def poll():
	return render_template('row.html', name1=cache.get('name1'), image1=cache.get('image1'), finished1=cache.get('finished1'), name2=cache.get('name2'), image2=cache.get('image2'), finished2=cache.get('finished2'))


@app.route('/save/1', methods=['POST'])
def save1():

	name=cache.get('name1')
	image=cache.get('image1')

	url = "https://api.imgur.com/3/image.json"
	headers = {
	    'content-type': "multipart/form-data; boundary=---011000010111000001101001",
	    'authorization': "Client-ID 6851efbe5a89371",
	    }
	payload = "-----011000010111000001101001\r\nContent-Disposition: form-data; name=\"image\"\r\n\r\n"+image+"\r\n-----011000010111000001101001\r\nContent-Disposition: form-data; name=\"title\"\r\n\r\n"+name+"\r\n-----011000010111000001101001\r\nContent-Disposition: form-data; name=\"description\"\r\n\r\nCreated with a Leap Motion at HackNC 2017\r\n-----011000010111000001101001\r\nContent-Disposition: form-data; name=\"type\"\r\n\r\nbase64\r\n-----011000010111000001101001--"

	response = requests.request("POST", url, data=payload, headers=headers)
	print(response.text)
	return redirect(response.json()["data"]["link"], code=302)

@app.route('/save/2', methods=['POST'])
def save2():

	name=cache.get('name2')
	image=cache.get('image2')

	url = "https://api.imgur.com/3/image.json"
	headers = {
	    'content-type': "multipart/form-data; boundary=---011000010111000001101001",
	    'authorization': "Client-ID 6851efbe5a89371",
	    }
	payload = "-----011000010111000001101001\r\nContent-Disposition: form-data; name=\"image\"\r\n\r\n"+image+"\r\n-----011000010111000001101001\r\nContent-Disposition: form-data; name=\"title\"\r\n\r\n"+name+"\r\n-----011000010111000001101001\r\nContent-Disposition: form-data; name=\"description\"\r\n\r\nCreated with a Leap Motion at HackNC 2017\r\n-----011000010111000001101001\r\nContent-Disposition: form-data; name=\"type\"\r\n\r\nbase64\r\n-----011000010111000001101001--"

	response = requests.request("POST", url, data=payload, headers=headers)
	print(response.text)
	return redirect(response.json()["data"]["link"], code=302)


# Ridiculously simplistic running mechanism
if __name__ == "__main__":
	app.run(host='0.0.0.0', port=settings.PORT, debug=True, threaded=True)
