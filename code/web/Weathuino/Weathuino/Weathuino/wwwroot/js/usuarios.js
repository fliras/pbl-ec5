function deletaRegistro(id) {
	const deveDeletar = confirm('Tem certeza que deseja excluir este registro?');
	if (deveDeletar)
		document.location.href = `/Usuarios/Delete?id=${id}`;
}

function filtraUsuarios() {
	const id = document.getElementById('id').value;
	const nome = document.getElementById('nome').value;
	const email = document.getElementById('email').value;

	$.ajax({
		url: "/usuarios/ConsultaComFiltros",
		data: { id, nome, email },
		success: function (dados) {
			if (dados.erro) {
				alert(dados.msg);
			}
			else {
				document.getElementById('dadosUsuarios').innerHTML = dados;
			}
		},
	});
}

window.onload = filtraUsuarios;