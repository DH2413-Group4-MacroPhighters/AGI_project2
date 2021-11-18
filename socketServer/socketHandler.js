const WebSocket = require("ws")
function Start(port){

    const wss = new WebSocket.Server({ port: port+1 },()=>{});
    
    wss.on('connection', socket=>{

        socket.on('message', (data) => {
            console.log('Websocket data received \n %o',data.toString());
            wss.clients.forEach((socket) => {

                socket.send(data.toString());
            });
        })
    })

    wss.on('listening',()=>{
        console.log('listening on websocket on: '+(port+1));
    })
}

module.exports = {
    Start: Start
};

