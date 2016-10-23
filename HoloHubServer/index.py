import json
import requests
from flask import Flask, url_for, request, Response
from soco import SoCo

app = Flask(__name__)

app.config.from_pyfile('settings.py')

sonos = SoCo(app.config['SPEAKER_IP'])



def get_track_image(artist, album):
    blank_image = url_for('static', filename='img/blank.jpg')

    headers = {
        "Accept-Encoding": "gzip"
    }
    req = requests.get(
        'http://santybm.com/api.php/?entity=album&country=us&query=' +
        album + ' ' + artist,
        headers=headers)

    if req.status_code != requests.codes.ok:
        return blank_image

    result = json.loads(req.content.decode('utf-8'))
    try:
        return result[0]['hires']
    except (KeyError, IndexError):
        return blank_image


@app.route("/play")
def play():
    sonos.play()
    resp = Response('Playing', status=200)
    return resp


@app.route("/pause")
def pause():
    sonos.pause()
    resp = Response('Paused', status=200)
    return resp


@app.route("/next")
def next():
    sonos.next()
    resp = Response('Skipped', status=200)
    return resp


@app.route("/previous")
def previous():
    sonos.previous()
    resp = Response('Replaying', status=200)
    return resp


@app.route("/volume")
def volume():
    try:
        volumeNumber = int(request.args.get('value'))
    except:
        resp = Response('Incorrect request format.', status=400)
        return resp

    if volumeNumber > 100:
        volumeNumber=100
    sonos.volume += volumeNumber
    resp = Response('Volume Changed', status=200)
    return resp

@app.route("/picture")
def picture():
    try:
        track = sonos.get_current_track_info()
        urlSend = track['album_art']
        # urlSend = get_track_image(track['artist'], track['album'])
        resp = Response(urlSend, status=200)
    except:
        blank_image = url_for('static', filename='img/blank.jpg')
        resp = Response(blank_image, status=200)
    return resp


@app.route("/")
def index():
    try:
        track = sonos.get_current_track_info()
        # urlSend = get_track_image(track['artist'], track['album'])
        resp = Response(json.dumps(track).encode('utf-8'), status=200)
    except:
        # blank_image = url_for('static', filename='img/blank.jpg')
        resp = Response("Not Started", status=200)
    return resp


if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0')