var token = "df"

function change(val) {
    token = val
}

async function getUser() {
    var tok = "Bearer " + token
    const r = await fetch('/api/Contacts2/user', {
        method: 'GET',
        headers: {
            'Authorization': tok,
            'Content-Type': 'application/json'
        }
    });
    const d = await r.json()
    return d;
}

async function getContacts() {
    var tok = "Bearer " + token
    const r = await fetch('/api/Contacts2/', {
        method: 'GET',
        headers: {
            'Authorization': tok,
            'Content-Type': 'application/json'
        }
    })
    const d = await r.json()
    return d
}

async function getContact(contName) {
    var tok = "Bearer " + token
    const r = await fetch('/api/Contacts2/' + contName, {
        method: 'GET',
        headers: {
            'Authorization': tok,
            'Content-Type': 'application/json'
        }
    })
    const d = await r.json()
    return d
}



async function printUser() {
    var user = await getUser()
    let elem = document.getElementById('main-user')
    elem.innerHTML += "<img src='/Images/img1.jpg' id='user-img' class='profileimage'>" +
        "<span clas='d-flex'>" + user.name + "</span>"
    $(async function () {
        var connection = new signalR.HubConnectionBuilder().withUrl("/myHub").build();

        connection.start();
        var user = await getUser()
        connection.on("ChangeReceived", async function (contname2, message, username, id) {
            console.log("fuck")
            if (contname2 === user.username) {
                writeMessage(contname2, message, username, id)
            }
        })
    })
}

async function postContact(uesrname, name, servert) {
    var p = location.host
    var s = await getUser()
    const d = await fetch('http://' + servert + '/api/Contacts/invitations', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ from: s.username, to: uesrname, server: p })
    });
    var tok = "Bearer " + token
    const r = await fetch('/api/Contacts2', {
        method: 'POST',
        headers: {
            'Authorization': tok,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ UserID: uesrname, Name: name, server: servert })
    });
}

async function postMessage(contName, message) {
    var s = await getUser()
    let c = await getContact(contName)
    c.server = String(c.server)
    const d = await fetch('http://' + c.server + '/api/Contacts/transfer', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ from: s.username, to: contName, content: message })
    });
    var tok = "Bearer " + token
    const r = await fetch('/api/Contacts2/' + contName + '/messages', {
        method: 'POST',
        headers: {
            'Authorization': tok,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ Content: message })
    });
    const dr = await r.json()
    return dr.id
}

async function addContact2(curr_user) {
    let username = document.getElementById('Username').value
    if ((username.trim()).length === 0) {
        return
    }
    let name = document.getElementById('Name').value
    if ((name.trim()).length === 0) {
        return
    }
    let server = document.getElementById('Server').value
    if ((server.trim()).length === 0) {
        return
    }
    document.getElementById('Username').value = ''
    document.getElementById('Name').value = ''
    document.getElementById('Server').value = ''

    await postContact(username, name, server)
    await printContacts2()
}

async function updateContact() {
    let new_name = document.getElementById('new-name').value
    if ((new_name.trim()).length === 0) {
        return
    }
    let new_server = document.getElementById('new-server').value
    if ((new_server.trim()).length === 0) {
        return
    }

    document.getElementById('new-name').value = ''
    document.getElementById('new-server').value = ''
    let curr_chat = document.getElementById('contact_name').innerText
    var tok = "Bearer " + token
    const r = await fetch('/api/Contacts2/' + curr_chat, {
        method: 'PUT',
        headers: {
            'Authorization': tok,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ Name: new_name, server: new_server })
    });
    document.getElementById(curr_chat + '-n').innerText = new_name
    document.getElementById(curr_chat + '-n2').innerText = new_name
}

async function removeContact() {
    let curr_chat = document.getElementById('contact_name').innerText
    var tok = "Bearer " + token
    const r = await fetch('/api/Contacts2/' + curr_chat, {
        method: 'DELETE',
        headers: {
            'Authorization': tok,
            'Content-Type': 'application/json'
        }
    });
    await printContacts2()
    let elem = document.getElementById('messages')
    elem.innerHTML = "<div class=\"row home h-100 align-items-center\">" +
        "<div class=\"centered\">" +
        "<img class=\"centert\" src=\"~/Images/whatsicon.png\" height=\"170px\" alt=\"\">" +
        "<h4 class=\"wellcome centert2\">Wellcome to whats app web app!! </h4>" +
        "</div>" +
        "</div>"
}



async function printContacts2() {
    let message = ''
    let time = ''
    let contacts = await getContacts()
    let str = ''
    contacts.forEach(cont => {
        if (cont.last == null) {
            message = ''
            time = ''
        } else {
            message = cont.last
            cont.lastdate = String(cont.lastdate)
            time = cont.lastdate.substring(11)
        }
        str += ("<div class=\"chat-list-item d-flex flex-row w-100 p-2 border-bottom\" onclick=\"showMessages2(" + "\'" + cont.contname + "\'" + ")\">" +
            "<div id='cont-img'>" +
            "<img src='/Images/img1.jpg' alt='Profile Photo' class='img-fluid rounded-circle mr-2' style=\"height:50px; max-width: 55px;\">" +
            "</div>" +
            "<div class=\"w-50\">" +
            "<h5 class=\"mb-1\" id=\"" + cont.contname + "-n\">" + cont.name + "</h5>" +
            "<p class=\"mb-1\" id=\"" + cont.contname + "-m\">" + message + "</p>" +
            "</div>" +
            "<div class=\"flex-grow-1 text-right\">" +
            "<div class=\"small time\" id=\"" + cont.contname + "-t\">" + time + "</div>" +
            "</div>" +
            "</div>")
    })
    str += "<table class='table table-bordered table-striped mb-0'>" + "</table>"
    document.getElementById('contact_div').innerHTML = str
}

async function showMessages2(contname) {
    let elem = document.getElementById('messages')
    let cont = await getContact(contname)
    elem.innerHTML = "<div class=\"bg-light\">" +
        "<img src='/Images/img1.jpg' class=\"profileimage\">" +
        "<span clas=\"d-flex\" id=\"" + cont.contname + "-n2\">" + cont.name + "</span>" +
        "<span id=\"contact_name\" clas=\"d-flex\" style=\"display: none;\">" + cont.contname + "</span>" +
        "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-search\" viewBox=\"0 0 16 16\">" +
        "<path d=\"M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z\"/>" +
        "</svg>" +
        "<button class=\"dropdown\">" +
        "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-three-dots-vertical\" viewBox=\"0 0 16 16\">" +
        "<path d=\"M9.5 13a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0zm0-5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0zm0-5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0z\"/>" +
        "</svg>" +
        "<div class=\"dropdown-content\">" +
        "<div class=\"biclips2\" data-bs-toggle=\"modal\" data-bs-target=\"#staticBackdrop4\">Info</div>" +
        "<div class=\"biclips2\" data-bs-toggle=\"modal\" data-bs-target=\"#staticBackdrop3\">Delete chat</div>" +
        "<div class=\"biclips2\">Mute</div>" +
        "</div>" +
        "</button>" +
        "</div>" +
        "<div class=\"table-wrapper-scroll-y my-custom-scrollbar d-flex flex-column chat-de  overlay\" id=\"chat_p\">" +
        readMessage2(cont.messages) +
        "<table class=\"table table-bordered table-striped mb-0\">" +
        "</table>" +
        "</div>" +
        "<div class=\"input-group Typehere n-input-group\">" +
        "<span class=\"input-group-text iconsandinput\">" +
        "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-emoji-laughing\" viewBox=\"0 0 16 16\">" +
        "<path d=\"M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z\"/>" +
        "<path d=\"M12.331 9.5a1 1 0 0 1 0 1A4.998 4.998 0 0 1 8 13a4.998 4.998 0 0 1-4.33-2.5A1 1 0 0 1 4.535 9h6.93a1 1 0 0 1 .866.5zM7 6.5c0 .828-.448 0-1 0s-1 .828-1 0S5.448 5 6 5s1 .672 1 1.5zm4 0c0 .828-.448 0-1 0s-1 .828-1 0S9.448 5 10 5s1 .672 1 1.5z\"/>" +
        "</svg>" +
        "<button class=\"dropup\">" +
        "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-paperclip\" viewBox=\"0 0 16 16\">" +
        "<path d=\"M4.5 3a2.5 2.5 0 0 1 5 0v9a1.5 1.5 0 0 1-3 0V5a.5.5 0 0 1 1 0v7a.5.5 0 0 0 1 0V3a1.5 1.5 0 1 0-3 0v9a2.5 2.5 0 0 0 5 0V5a.5.5 0 0 1 1 0v7a3.5 3.5 0 1 1-7 0V3z\"/>" +
        "</svg>" +
        "<div class=\"dropup-content\">" +
        "<div class=\"clipdivs\">" +
        "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-person biclips\" viewBox=\"0 0 16 16\">" +
        "<path d=\"M8 8a3 3 0 1 0 0-6 3 3 0 0 0 0 6zm2-3a2 2 0 1 1-4 0 2 2 0 0 1 4 0zm4 8c0 1-1 1-1 1H3s-1 0-1-1 1-4 6-4 6 3 6 4zm-1-.004c-.001-.246-.154-.986-.832-1.664C11.516 10.68 10.289 10 8 10c-2.29 0-3.516.68-4.168 1.332-.678.678-.83 1.418-.832 1.664h10z\"/>" +
        "</svg>" +
        "</div>" +
        "<div>" +
        "<svg xmlns=\"http://www.w3.org/2000/svg\"  width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-mic-fill biclips\" viewBox=\"0 0 16 16\">" +
        "<path d=\"M5 3a3 3 0 0 1 6 0v5a3 3 0 0 1-6 0V3z\"/>" +
        "<path d=\"M3.5 6.5A.5.5 0 0 1 4 7v1a4 4 0 0 0 8 0V7a.5.5 0 0 1 1 0v1a5 5 0 0 1-4.5 4.975V15h3a.5.5 0 0 1 0 1h-7a.5.5 0 0 1 0-1h3v-2.025A5 5 0 0 1 3 8V7a.5.5 0 0 1 .5-.5z\"/>" +
        "</svg>" +
        "</div>" +
        "<div>" +
        "<svg data-bs-toggle=\"modal\" data-bs-target=\"#staticBackdrop3\" xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-camera biclips\" viewBox=\"0 0 16 16\">" +
        "<path d=\"M15 12a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1V6a1 1 0 0 1 1-1h1.172a3 3 0 0 0 2.12-.879l.83-.828A1 1 0 0 1 6.827 3h2.344a1 1 0 0 1 .707.293l.828.828A3 3 0 0 0 12.828 5H14a1 1 0 0 1 1 1v6zM2 4a2 2 0 0 0-2 2v6a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2h-1.172a2 2 0 0 1-1.414-.586l-.828-.828A2 2 0 0 0 9.172 2H6.828a2 2 0 0 0-1.414.586l-.828.828A2 2 0 0 1 3.172 4H2z\"/>" +
        "<path d=\"M8 11a2.5 2.5 0 1 1 0-5 2.5 2.5 0 0 1 0 5zm0 1a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7zM3 6.5a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0z\"/>" +
        "</svg>" +
        "</div>" +
        "<div>" +
        "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-image biclips\" viewBox=\"0 0 16 16\" data-bs-toggle=\"modal\" data-bs-target=\"#staticBackdrop2\">" +
        "<path d=\"M6.002 5.5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0z\"/>" +
        "<path d=\"M2.002 1a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V3a2 2 0 0 0-2-2h-12zm12 1a1 1 0 0 1 1 1v6.5l-3.777-1.947a.5.5 0 0 0-.577.093l-3.71 3.71-2.66-1.772a.5.5 0 0 0-.63.062L1.002 12V3a1 1 0 0 1 1-1h12z\"/>" +
        "</svg>" +
        "</div>" +
        "</div>" +
        "</button>" +
        "<input type=\"text\" class=\"form-control\" aria-label=\"Amount (to the nearest dollar)\" placeholder=\"Type a message\" id=\"typem\">" +
        "<div id=\"send-btn\" >" +
        "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-send-fill\" viewBox=\"0 0 16 16\">" +
        "<path d=\"M15.964.686a.5.5 0 0 0-.65-.65L.767 5.855H.766l-.452.18a.5.5 0 0 0-.082.887l.41.26.001.002 4.995 3.178 3.178 4.995.002.002.26.41a.5.5 0 0 0 .886-.083l6-15Zm-1.833 1.89L6.637 10.07l-.215-.338a.5.5 0 0 0-.154-.154l-.338-.215 7.494-7.494 1.178-.471-.47 1.178Z\">" +
        "</svg>" +
        "</div>" +
        "</span>" +
        "</div>"
    const wage = document.getElementById('typem');
    wage.addEventListener('keydown', (e) => {
        if (e.key === 'Enter') {
            sendMessage2(contname);
        }
    });
    $("#send-btn").click(async () => {
        let message = document.getElementById('typem').value
        await sendMessage2(contname)
    });
}

async function sendMessage2(contname) {
    let message = document.getElementById('typem').value
    document.getElementById('typem').value = ''
    var today = new Date();
    var time = today.getHours() + ":" + today.getMinutes();
    if ((message.trim()).length === 0) {
        return
    } else {
        let id = await postMessage(contname, message.trim())
        let elem = document.getElementById('chat_p')
        elem.innerHTML += "<div class=\"flex-row d-flex align-self-end self p-1 my-1 mx-3 rounded shadow-sm message-item greenbackground\">" +
                            "<div class='options'>" +
                                "<svg onclick='set_mid(" + id + ")' xmlns='http://www.w3.org/2000/svg' data-bs-toggle=\"modal\" data-bs-target=\"#staticBackdrop5\" fill='currentColor' class='bi bi-trash del-icon' viewBox='0 0 16 16'>" +
                                    "<path d='M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z' />" +
                                    "<path fill-rule='evenodd' d='M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z' />" +
                                "</svg>" +
                                "<svg onclick='set_mid(" + id + ")' xmlns='http://www.w3.org/2000/svg' data-bs-toggle=\"modal\" data-bs-target=\"#staticBackdrop6\" fill='currentColor' class='bi bi-pencil del-icon' viewBox='0 0 16 16'>" +
                                    "<path d='M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708l-10 10a.5.5 0 0 1-.168.11l-5 2a.5.5 0 0 1-.65-.65l2-5a.5.5 0 0 1 .11-.168l10-10zM11.207 2.5 13.5 4.793 14.793 3.5 12.5 1.207 11.207 2.5zm1.586 3L10.5 3.207 4 9.707V10h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.293l6.5-6.5zm-9.761 5.175-.106.106-1.528 3.821 3.821-1.528.106-.106A.5.5 0 0 1 5 12.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.468-.325z' />" +
                                "</svg>" +
                            "</div>" +
                            "<div class=\"d-flex flex-row\">" +
                                "<div class=\"body m-1 mr-2\" id=\"" + (id) + "-m\">" + message + "</div>" +
                                "<div class=\"time ml-auto small text-right flex-shrink-0 align-self-end text-muted\" style=\"width:75px;\">" +
                                time +
                                "<i class=\"fas fa-check-circle\"></i>" +
                                "</div>" +
                            "</div>" +
                        "</div>"
        document.getElementById(contname + '-t').innerText = time
        document.getElementById(contname + '-m').innerText = message.substr(0, 20)
    }
}

function writeMessage(contname, message, username, id) {
    console.log(contname, username)
    var today = new Date();
    var time = today.getHours() + ":" + today.getMinutes();
    let elem = document.getElementById('chat_p')
    let curr_chat = document.getElementById('contact_name').innerText
    console.log(curr_chat)
    if (curr_chat === username) {
        elem.innerHTML += "<div class=\"flex-row d-flex align-self-start self p-1 my-1 mx-3 rounded shadow-sm message-item bg-white\">" +
                            "<div class='options'>" +
                                "<svg onclick='set_mid(" + id + ")' xmlns='http://www.w3.org/2000/svg' data-bs-toggle=\"modal\" data-bs-target=\"#staticBackdrop5\" fill='currentColor' class='bi bi-trash del-icon' viewBox='0 0 16 16'>" +
                                    "<path d='M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z' />" +
                                    "<path fill-rule='evenodd' d='M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z' />" +
                                "</svg>" +
                            "</div>" +
                            "<div class=\"d-flex flex-row\">" +
                                "<div class=\"body m-1 mr-2\">" + message + "</div>" +
                                "<div class=\"time ml-auto small text-right flex-shrink-0 align-self-end text-muted\" style=\"width:75px;\">" +
                                time +
                                "<i class=\"fas fa-check-circle\"></i>" +
                                "</div>" +
                            "</div>" +
                        "</div>"
    }
    document.getElementById(username + '-t').innerText = time
    document.getElementById(username + '-m').innerText = message.substr(0, 20)
}
var mid = 0

function set_mid(id) {
    mid = id
}
function readMessage2(messages) {
    let str = ''
    i = 0
    let side = ''
    let color = ''
    let time = ''
    let delet = "<svg onclick='set_mid(" + i + ")' xmlns='http://www.w3.org/2000/svg' data-bs-toggle=\"modal\" data-bs-target=\"#staticBackdrop5\" fill='currentColor' class='bi bi-trash del-icon' viewBox='0 0 16 16'>" +
        "<path d='M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z' />" +
        "<path fill-rule='evenodd' d='M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z' />" +
        "</svg>"
    let edit = ''
    messages.forEach(msg => {
        msg.created = String(msg.created)
        time = msg.created.substring(11)
        delet = "<svg onclick='set_mid(" + msg.id + ")' xmlns='http://www.w3.org/2000/svg' data-bs-toggle=\"modal\" data-bs-target=\"#staticBackdrop5\" fill='currentColor' class='bi bi-trash del-icon' viewBox='0 0 16 16'>" +
            "<path d='M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z' />" +
            "<path fill-rule='evenodd' d='M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z' />" +
            "</svg>"
        if (msg.sent === false) {
            side = 'start'
            color = 'bg-white'
            edit = ''
        } else {
            side = 'end'
            color = 'greenbackground'
            edit = "<svg onclick='set_mid(" + msg.id + ")' xmlns='http://www.w3.org/2000/svg' data-bs-toggle=\"modal\" data-bs-target=\"#staticBackdrop6\" fill='currentColor' class='bi bi-pencil del-icon' viewBox='0 0 16 16'>" +
                "<path d='M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708l-10 10a.5.5 0 0 1-.168.11l-5 2a.5.5 0 0 1-.65-.65l2-5a.5.5 0 0 1 .11-.168l10-10zM11.207 2.5 13.5 4.793 14.793 3.5 12.5 1.207 11.207 2.5zm1.586 3L10.5 3.207 4 9.707V10h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.293l6.5-6.5zm-9.761 5.175-.106.106-1.528 3.821 3.821-1.528.106-.106A.5.5 0 0 1 5 12.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.468-.325z' />" +
                "</svg>"
        }
        str += "<div class=\"flex-row d-flex align-self-" + side + " self p-1 my-1 mx-3 rounded shadow-sm message-item " + color + "\">" +
            "<div class='options'>" +
            delet +
            edit +
            "</div>" +
            "<div class=\"d-flex flex-row\">" +
            "<div class=\"body m-1 mr-2\" id=\"" + (msg.id) + "-m\">" + msg.content + "</div>" +
            "<div class=\"time ml-auto small text-right flex-shrink-0 align-self-end text-muted\" style=\"width:75px;\">" +
            time +
            "<i class=\"fas fa-check-circle\"></i>" +
            "</div>" +
            "</div>" +
            "</div>"
        i++
    });
    return str
}


async function removeMessage() {
    let curr_chat = document.getElementById('contact_name').innerText
    var tok = "Bearer " + token
    const r = await fetch('/api/Contacts2/' + curr_chat + '/messages/' + mid, {
        method: 'DELETE',
        headers: {
            'Authorization': tok,
            'Content-Type': 'application/json'
        }
    });
    showMessages2(curr_chat)
}

async function updateMessage() {
    let new_content = document.getElementById('new-content').value
    if ((new_content.trim()).length === 0) {
        return
    }
    document.getElementById('new-content').value = ''
    let curr_chat = document.getElementById('contact_name').innerText
    var tok = "Bearer " + token
    const r = await fetch('/api/Contacts2/' + curr_chat + '/messages/' + mid, {
        method: 'PUT',
        headers: {
            'Authorization': tok,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ Content: new_content })
    });
    document.getElementById(mid + "-m").innerText = new_content
}