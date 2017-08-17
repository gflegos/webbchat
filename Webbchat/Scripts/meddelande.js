function säkraHtml(indata) {
	return indata
		.replace(/&/g, "&amp;")
		.replace(/</g, "&lt;")
		.replace(/>/g, "&gt;")
		.replace(/"/g, "&quot;")
		.replace(/'/g, "&#039;");
}

function uppdateraMeddelanden() {
	if(Date.now() < (window.senasteAktivitet + 300000)) {
		$('#inaktiv').css('display', 'none');
		hämtaMeddelanden();
	}
	else {
		$('#inaktiv').css('display', 'inline-block');
		loggaUt();
	}
	$('#meddelande-fält').prop('disabled', false);
	$('#meddelande-skicka').prop('disabled', false);
}

function hämtaMeddelanden() {
	var sistaId = $('.meddelande:last').attr('data-meddelandeid');
	if(sistaId == null)
		sistaId = 0;
	var jwt = Cookies.get('webbchatToken');
	$.ajax({
		url: "Home/HämtaMeddelanden",
		data: {
			token: jwt,
			meddelandeId: sistaId
		},
		dataType: "json",
		method: "POST",
		success: function(data) {
			var tid;
			var meddelande;
			var botten = false;
			if($('#chat-fält').scrollTop() >= ($('#chat-fält')[0].scrollHeight - $('#chat-fält').height()))
				botten = true;
			for(i = data.length - 1; i >= 0; i--) {
				tid = new Date(data[i].tid * 1000).toLocaleString();
				//meddelande = säkraHtml(tid.toLocaleString() + " " + data[i].användarnamn + " " + data[i].text);
				meddelande = "<tr class=\"meddelande\" data-meddelandeid=\"" + data[i].id + "\">";
				meddelande += "<td class=\"meddelande-tid\">" + tid + "</td>";
				meddelande += "<td class=\"meddelande-avsändare\">" + data[i].användarnamn + "</td>";
				meddelande += "<td class=\"meddelande-text\">" + säkraHtml(data[i].text) + "</td>";
				$('#chat-tabell').append(meddelande);
			}
			if(botten)
				$('#chat-fält').scrollTop($('#chat-fält')[0].scrollHeight);
		}
	});
}

function skickaMeddelande() {
	$('#meddelande-bekräftelse').css('display', 'none');
	$('#meddelande-fel').css('display', 'none');
	var jwt = Cookies.get('webbchatToken');
	var meddelande = $('#meddelande-fält').val();
	$.ajax({
		url: "Home/SkickaMeddelande",
		data: {
			token: jwt,
			innehåll: meddelande
		},
		dataType: "json",
		method: "POST",
		success: function(data) {
			if(data.lyckades) {
				$('#meddelande-fält').prop('disabled', true);
				$('#meddelande-fält').val("");
				$('#meddelande-skicka').prop('disabled', true);
				//$('#meddelande-bekräftelse').css('display', 'inline-block');
			}
			else {
				$('#meddelande-fel').css('display', 'inline-block');
			}
		}
	});
}

$(document).ready(function() {
	window.senasteAktivitet = Date.now();
	$('#meddelande-fält').keypress(function(event) {
		if(event.which == 13 && !event.shiftKey) {
			event.preventDefault();
			window.senasteAktivitet = Date.now();
			if($('#meddelande-fält').val() !== "")
				skickaMeddelande();
		}
	});
	setInterval(function() {
		uppdateraMeddelanden();
	}, 1000);
	$('#chat-fält').scrollTop($('#chat-fält')[0].scrollHeight);
});