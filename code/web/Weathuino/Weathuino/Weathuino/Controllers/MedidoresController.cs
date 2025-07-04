﻿using System;
using Microsoft.AspNetCore.Mvc;
using Weathuino.DAO;
using Weathuino.Models;
using Weathuino.Enums;
using Weathuino.APIs.Fiware;
using Weathuino.APIs.Fiware.Models;

namespace Weathuino.Controllers
{
    public class MedidoresController : CRUDController<MedidorViewModel>
    {
        public MedidoresController()
        {
            DAO = new MedidorDAO();
            AcessoExigido = PerfisAcesso.COMUM;
        }

        protected override bool ExecuteBeforeSave(MedidorViewModel model, ModosOperacao operacao)
        {
            if (operacao != ModosOperacao.INCLUSAO)
                return true;

            FiwareClient fClient = new FiwareClient();
            FiwareOutput fOutput;

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
