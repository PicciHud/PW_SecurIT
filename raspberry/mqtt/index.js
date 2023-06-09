const SerialPort = require('serialport')
const Readline = require('@serialport/parser-readline')
const port = new SerialPort('com7') // com0com di Windwos
const mqtt = require('mqtt')
const client = mqtt.connect('mqtt://192.168.0.222', { username: "mario", password: "davide" }) //password non in chiaro...

const parser = port.pipe(new Readline({ delimiter: '\r\n' }))
parser.on('data', parseMSg)

// passare i dati via mqtt, per poterli leggere tramite node-red
// trasformare il dato in json

client.on('connect', function () {
    console.log("Connected to its_test")
})

function parseMSg(data) {
    let values = data.split('\n')
    //console.log(values)
    let jsonOut = {
        "codepic": values[0]//,
        //"rasp_id": values[1],
        //"home_id": values[2]
    }
    let jsonStr = JSON.stringify(jsonOut)
    console.log(jsonStr)
    client.publish('/its_test', jsonStr)
}

// Ricezione

client.on('connect', function () {
    console.log("Connected to its_out")
    client.subscribe('/its_out') // Sottoscrivi al topic '/its_out'
})

client.on('message', function (topic, message) {
    console.log(message.toString());
});