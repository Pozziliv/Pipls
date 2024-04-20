import zmq
import json
import subprocess
from Constants import *  # false and true

serversPath = input()

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")

ports = {"Ports" : [], "Players" : [] }
portsToSet = []
portsIndex = 0

def startNewServer():
    global portsIndex

    subprocess.Popen(serversPath)
    portCreated = false

    while portCreated == false:
        createdPort = str(7777 + portsIndex)
        if (createdPort not in ports["Ports"]):
            ports["Ports"].append(createdPort)
            ports["Players"].append(0)
            portsToSet.append(createdPort)
            portCreated = true
        else:
            portsIndex += 1

    portsIndex += 1
    socket.send(createdPort.encode())

def getPort():
    socket.send(portsToSet.pop(0).encode())

def deletePort(port):
    index = ports["Ports"].index(port)
    ports["Ports"].pop(index)
    ports["Players"].pop(index)

def setPlayersCountAtGame(port, count):
    index = ports["Ports"].index(port)
    ports["Players"][index] = int(count)

def setPlayersCountPerGame():
    jsonData = json.dumps(ports)
    socket.send(jsonData.encode())

while True:
    message = socket.recv()
    decodedMessage = message.decode()
    print("Received request: %s" % decodedMessage)

    if (decodedMessage == "StartNewServer"):
        startNewServer()
        continue

    if (decodedMessage == "GetPort"):
        getPort()
        continue

    if("SetPlayers" in decodedMessage):
        port, count = decodedMessage.replace("SetPlayers ", "").split(" ")
        setPlayersCountAtGame(port, count)

    if(decodedMessage == "GetServersData"):
        setPlayersCountPerGame()
        continue

    if("DeleteServer" in decodedMessage):
        port = decodedMessage.replace("DeleteServer ", "")
        deletePort(port)

    socket.send(b"Gotten")
