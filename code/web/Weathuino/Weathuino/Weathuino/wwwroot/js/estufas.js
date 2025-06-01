function deletaRegistro(id) {
	const deveDeletar = confirm('Tem certeza que deseja excluir este registro?');
	if (deveDeletar)
		document.location.href = `/Estufas/Delete?id=${id}`;
}

function filtraEstufas() {
	const id = document.getElementById('id').value;
	const nome = document.getElementById('nome').value;

	$.ajax({
		url: "/estufas/ConsultaComFiltros",
		data: { id, nome },
		success: function (dados) {
			if (dados.erro) {
				alert(dados.msg);
			}
			else {
				document.getElementById('dadosEstufas').innerHTML = dados;
			}
		},
	});
}

window.onload = filtraEstufas;