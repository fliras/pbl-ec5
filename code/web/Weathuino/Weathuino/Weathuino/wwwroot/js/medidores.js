function deletaMedidor(id) {
	const deveDeletar = confirm('Tem certeza que deseja excluir este medidor?');
	if (deveDeletar)
		document.location.href = `/Medidores/Delete?id=${id}`;
}

function filtraMedidores() {
	const id = document.getElementById('id').value;
	const nome = document.getElementById('nome').value;
	$.ajax({
		url: "/medidores/ConsultaComFiltros",
		data: { id, nome },
		success: function (dados) {
			if (dados.erro) {
				alert(dados.msg);
			}
			else {
				document.getElementById('dadosMedidores').innerHTML = dados;
			}
		},
	});
}

window.onload = filtraMedidores;