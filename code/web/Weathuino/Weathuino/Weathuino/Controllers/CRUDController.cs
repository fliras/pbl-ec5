using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using Weathuino.DAO;
using Weathuino.Enums;
using Weathuino.Models;

namespace Weathuino.Controllers
{
    public abstract class CRUDController<T> : PadraoController where T : PadraoViewModel
    {
        protected PadraoDAO<T> DAO { get; set; }
        protected bool GeraProximoId { get; set; } = true;
        protected string NomeViewIndex { get; set; } = "index";
        protected string NomeViewForm { get; set; } = "form";

        public override IActionResult Index()
        {
            try
            {
                var lista = DAO.ObtemTodos();
                return View(NomeViewIndex, lista);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public virtual IActionResult Create()
        {
            try
            {
                ViewBag.Operacao = ModosOperacao.INCLUSAO;
                T model = Activator.CreateInstance<T>();
                PreencheDadosParaView(ModosOperacao.INCLUSAO, model);
                return View(NomeViewForm, model);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        protected virtual void PreencheDadosParaView(ModosOperacao Operacao, T model)
        {
            if (GeraProximoId && Operacao == ModosOperacao.INCLUSAO)
                model.Id = DAO.GeraProximoID();
        }

        public virtual IActionResult Save(T model, ModosOperacao Operacao)
        {
            try
            {
                if (!ValidaDados(model, Operacao))
                {
                    ViewBag.Operacao = Operacao;
                    PreencheDadosParaView(Operacao, model);
                    return View(NomeViewForm, model);
                }

                if (Operacao == ModosOperacao.INCLUSAO)
                    DAO.Insert(model);
                else
                    DAO.Update(model);

                return RedirectToAction(NomeViewIndex);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        // Atualmente não há validações comuns para todas as controllers, apenas a rotina de limpar o ModelState.
        protected virtual bool ValidaDados(T model, ModosOperacao operacao)
        {
            ModelState.Clear();
            return true;
        }

        public IActionResult Edit(int id)
        {
            try
            {
                ViewBag.Operacao = ModosOperacao.ALTERACAO;
                var model = DAO.ObtemPorID(id);
                if (model == null)
                    return RedirectToAction(NomeViewIndex);
                else
                {
                    PreencheDadosParaView(ModosOperacao.ALTERACAO, model);
                    return View(NomeViewForm, model);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult View(int id)
        {
            try
            {
                ViewBag.Operacao = ModosOperacao.VISUALIZACAO;
                var model = DAO.ObtemPorID(id);
                if (model == null)
                    return RedirectToAction(NomeViewIndex);
                else
                {
                    PreencheDadosParaView(ModosOperacao.VISUALIZACAO, model);
                    return View(NomeViewForm, model);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public virtual IActionResult Delete(int id)
        {
            try
            {
                DAO.Delete(id);
                return RedirectToAction(NomeViewIndex);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }

        }

        public IActionResult ConsultaComFiltros(FiltrosViewModel filtros)
        {
            try
            {
                List<T> registrosFiltrados = DAO.ConsultaComFiltros(filtros);
                return PartialView("pvGrid", registrosFiltrados);
            }
            catch (Exception error)
            {
                return Json(new { erro = true, msg = error.Message });
            }
        }
    }
}
