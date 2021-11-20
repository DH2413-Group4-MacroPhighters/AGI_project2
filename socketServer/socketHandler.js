const WebSocket = require("ws");
function Start(port){

    const wss = new WebSocket.Server({ port: port+1 },()=>{});
    
    wss.on('connection', socket=>{
        console.log('New client connected')
        socket.on('message', (data) => {
            console.log('Websocket data received \n %o',data.toString());
            wss.clients.forEach((socket) => {

                socket.send(data.toString());
            });
        })

        socket.on('close', () => {
            console.log("client has disconnected")
        });
    })

    wss.on('listening',()=>{
        console.log('listening on websocket on: '+(port+1));
    })
}

module.exports = {
    Start: Start
};

