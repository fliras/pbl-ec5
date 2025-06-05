using System;
using Microsoft.AspNetCore.Mvc;
using Weathuino.DAO;
using Weathuino.Models;
using Weathuino.Enums;
using Weathuino.APIs.Fiware;
using Weathuino.APIs.Fiware.Models;

namespace Weathuino.Controllers
{
    /// <summary>
    /// Gerencia o CRUD de medidores
    /// </summary>
    public class MedidoresController : CRUDController<MedidorViewModel>
    {
        public MedidoresController()
        {
            DAO = new MedidorDAO();
            AcessoExigido = PerfisAcesso.COMUM; // ao menos usuários logados devem acessar
        }

        /// <summary>
        /// Implementa as regras executadas antes do Save dos dados, em especial o registro do dispositivo no Fiware
        /// </summary>
        /// <param name="model"></param>
        /// <param name="operacao"></param>
        /// <returns></returns>
        protected override bool ExecuteBeforeSave(MedidorViewModel model, ModosOperacao operacao)
        {
            // apenas em inclusões esse método deve seguir
            if (operacao != ModosOperacao.INCLUSAO)
                return true;

            FiwareClient fClient = new FiwareClient();
            FiwareOutput fOutput;

            // Caso alguma requisição ao Fiware dê errado, um erro em tela é exibido
            fOutput = fClient.CriaDispositivo(model.DeviceIdFiware, model.Id);
            if (!fOutput.Sucesso)
            {
                ViewBag.ErrorAfterSave = fOutput.MensagemDeErro;
                return false;
            }

            fOutput = fClient.RegistraAtributosDeDispositivo(model.Id);
            if (!fOutput.Sucesso)
            {
                ViewBag.ErrorAfterSave = fOutput.MensagemDeErro;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Implementa uma regra de Delete customizada, por conta das interações com o Fiware
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IActionResult Delete(int id)
        {
            try
            {
                MedidorDAO dao = new MedidorDAO();
                bool medidorEmUso = dao.VerificaSeMedidorEstaEmUso(id);
                if (medidorEmUso)
                {
                    ViewBag.AlertaErro = "Este medidor não pode ser excluido pois está sendo utilizado por uma estufa";
                    return View("Index", dao.ObtemTodos());
                }

                MedidorViewModel medidor = dao.ObtemPorID(id);
                bool sucessoNaExclusaoDoFiware = DeletaMedidorNoFiware(medidor);
                if (!sucessoNaExclusaoDoFiware)
                {
                    return View("Index", dao.ObtemTodos());
                }

                dao.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        /// <summary>
        /// Realiza a exclusão de dispositivos no Fiware
        /// </summary>
        /// <param name="medidor"></param>
        /// <returns></returns>
        private bool DeletaMedidorNoFiware(MedidorViewModel medidor)
        {
            FiwareClient fClient = new FiwareClient();
            FiwareOutput fOutput = fClient.DeletaDispositivoNoAgentMQTT(medidor.DeviceIdFiware);

            if (!fOutput.Sucesso)
            {
                ViewBag.AlertaErro = fOutput.MensagemDeErro;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Implementa regras de validação customizadas para os medidores
        /// </summary>
        /// <param name="medidor"></param>
        /// <param name="operacao"></param>
        /// <returns></returns>
        protected override bool ValidaDados(MedidorViewModel medidor, ModosOperacao operacao)
        {
            bool validacaoBaseOK = base.ValidaDados(medidor, operacao);
            if (!validacaoBaseOK) return false;

            if (string.IsNullOrEmpty(medidor.DeviceIdFiware))
                ModelState.AddModelError("DeviceIdFiware", "Informe o DeviceID do dispositivo no Fiware");
            else
            {
                MedidorViewModel medidorDoDeviceID = ((MedidorDAO)DAO).ObtemPorDeviceID(medidor.DeviceIdFiware);
                bool deviceIdEmUso = medidorDoDeviceID != null && medidorDoDeviceID.Id != medidor.Id;
                if (deviceIdEmUso)
                    ModelState.AddModelError("DeviceIdFiware", "Este DeviceID já está em uso!");
            }

            if (string.IsNullOrEmpty(medidor.Nome))
                ModelState.AddModelError("Nome", "Informe um nome!");

            return ModelState.IsValid;
        }
    }
}
