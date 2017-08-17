function uppdateraKonto() {
	var jwt = Cookies.get('webbchatToken');
	if(jwt) {
		$.ajax({
			url: "Home/UppdateraKonto",
			data: {
				jwt: jwt
			},
			dataType: "json",
			method: "POST",
			success: function(data) {
				if(data.lyckades) {
					$('#inloggad-användarnamn').html(data.användarnamn);
				}
				else {
					loggaUt();
				}
			}
		});
		$('#logga-in').css('display', 'none');
		$('#inloggad').css('display', 'inline-block');
	}
	else {
		$('#inloggad-användarnamn').html("");
		$('#inloggad').css('display', 'none');
		$('#logga-in').css('display', 'inline-block');
	}
}

function loggaIn() {
	$('#logga-in-bekräftelse').css('display', 'none');
	$('#logga-in-fel').css('display', 'none');
	$.ajax({
		url: "Home/LoggaIn",
		data: {
			användarnamn: $('#logga-in-användarnamn-fält').val(),
			lösenord: $('#logga-in-lösenord-fält').val()
		},
		dataType: "json",
		method: "POST",
		success: function(data) {
			if(data.lyckades) {
				Cookies.set("webbchatToken", data.token);
				$('#logga-in-bekräftelse').css('display', 'inline-block');
			}
			else {
				$('#logga-in-fel').css('display', 'inline-block');
			}
			
		}
	});
}

function loggaUt() {
	Cookies.remove("webbchatToken");
	uppdateraKonto();
}

function registrera() {
	$('#registrera-bekräftelse').css('display', 'none');
	$('#registrera-fel').css('display', 'none');
	$.ajax({
		url: "Home/Registrera",
		data: {
			användarnamn: $('#registrera-användarnamn-fält').val(),
			lösenord: $('#registrera-lösenord-fält').val()
		},
		dataType: "json",
		method: "POST",
		success: function(data) {
			if(data.lyckades) {
				$('#registrera-bekräftelse').css('display', 'inline-block');
			}
			else {
				$('#registrera-fel').css('display', 'inline-block');
			}
		}
	});
}

$(document).ready(function() {
	$('#registrera-skicka').click(function(event) {
		event.preventDefault();
		registrera();
	});
	$('#logga-in-skicka').click(function(event) {
		event.preventDefault();
		window.senasteAktivitet = Date.now();
		loggaIn();
	});
	$('#logga-ut').click(function(event) {
		event.preventDefault();
		loggaUt();
	});
	$('#konto-fönster-öppna').click(function(event) {
		event.preventDefault();
		if($('#konto-fönster').css('display') == 'block')
			$('#konto-fönster').css('display', 'none');
		else
			$('#konto-fönster').css('display', 'block');
	});
	uppdateraKonto();
});
