function montaInstanciaDeGrafico(ctx, objPontosAcumulados, objAnnotations) {
	return new Chart(ctx, {
		type: 'line',
		data: {
			datasets: [
				{
					label: 'Temperatura (\u00B0C)',
					data: objPontosAcumulados,
					borderColor: 'rgba(75, 192, 192, 1)',
					backgroundColor: 'rgba(75, 192, 192, 0.2)',
					tension: 0.1
				},
			]
		},
		options: {
			scales: {
				x: {
					type: 'time',
					time: {
						tooltipFormat: 'dd/MM/yyyy HH:mm:ss',
						displayFormats: {
							second: 'dd/MM/yyyy HH:mm:ss',
							minute: 'dd/MM/yyyy HH:mm:ss',
							hour: 'dd/MM/yyyy HH:mm:ss'
						}
					},
					title: {
						display: true,
						text: 'Data e Hora'
					}
				},
				y: {
					title: {
						display: true,
						text: 'Temperatura (\u00B0C)'
					}
				}
			},
			plugins: {
				annotation: {
					annotations: objAnnotations
				},
				legend: {
					display: true
				}
			},
		}
	});
}

// plota duas linhas fixas no gráfico que representam as temperaturas mínimas e máximas da estufa
function montaRangesTemperatura(objAnnotations, tempMinima, tempMaxima) {
	objAnnotations.tempMinima = {
		type: 'line',
		yMin: tempMinima,
		yMax: tempMinima, // linha horizontal no Y = 30
		borderColor: 'red',
		borderWidth: 2,
		label: {
			display: true,
			content: `Temperatura Min.: ${tempMinima}\u00B0C`,
			position: 'end',
			backgroundColor: 'rgba(255,0,0,0.2)',
			color: 'red',
			yAdjust: -10
		}
	}
	objAnnotations.tempMaxima = {
		type: 'line',
		yMin: tempMaxima,
		yMax: tempMaxima, // linha horizontal no Y = 30
		borderColor: 'red',
		borderWidth: 2,
		label: {
			display: true,
			content: `Temperatura Max.: ${tempMaxima}\u00B0C`,
			position: 'end',
			backgroundColor: 'rgba(255,0,0,0.2)',
			color: 'red',
		}
	}
}

function montaAnnotationDoSetPoint(valorSetPoint) {
	return {
		type: 'line',
		yMin: valorSetPoint,
		yMax: valorSetPoint, // linha horizontal no Y = 30
		borderColor: 'red',
		borderWidth: 2,
		label: {
			display: true,
			content: `SP.: ${valorSetPoint}\u00B0C`,
			position: 'end',
			backgroundColor: 'rgba(255,0,0,0.2)',
			color: 'red',
			yAdjust: -10
		}
	}
}

function formataTimestamp(datahora) {
	return datahora.toLocaleString('pt-BR', {
		day: '2-digit',
		month: '2-digit',
		year: 'numeric',
		hour: '2-digit',
		minute: '2-digit',
		second: '2-digit'
	}).replace(',', '');
}

window.onload = () => {
	montaChart();
}