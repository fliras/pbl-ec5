using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Weathuino.DAO;
using Weathuino.Enums;
using Weathuino.Models;

namespace Weathuino.Controllers
{
    /// <summary>
    /// Classe base para as Controllers de CRUD do sistema
    /// </summary>
    /// <typeparam name="T">Tipo genérico que representa as models associadas a cada controller</typeparam>
    public abstract class CRUDController<T> : PadraoController where T : PadraoViewModel
    {
        protected PadraoDAO<T> DAO { get; set; }
        protected bool GeraProximoId { get; set; } = true; // Indica se a Controller deve gerenciar os IDs dos novos registros
        protected string NomeViewIndex { get; set; } = "index"; // Indica nome da View inicial da controller
        protected string NomeViewForm { get; set; } = "form"; // Nome da View de formulário da controller

        /// <summary>
        /// Monta a página principal da Controller, listando os dados
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Realiza a lógica base de criação do CRUD
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Prepara controles que devem ser carregados para o funcionamento da Controller. Pode ser modificado
        /// </summary>
        /// <param name="Operacao">Tipo de operação que o CRUD realizará na requisição</param>
        /// <param name="model">model com os dados da requisição</param>
        protected virtual void PreencheDadosParaView(ModosOperacao Operacao, T model)
        {
            if (GeraProximoId && Operacao == ModosOperacao.INCLUSAO)
                model.Id = DAO.GeraProximoID();
        }

        /// <summary>
        /// Implementa lógica base de salvamento de registros, tanto criação quanto edição.
        /// </summary>
        /// <param name="model">dados da requisição</param>
        /// <param name="Operacao">tipo de operação realizada</param>
        /// <returns></returns>
        public virtual IActionResult Save(T model, ModosOperacao Operacao)
        {
            try
            {
                // Se os dados são inválidos, a requisição é interrompida
                if (!ValidaDados(model, Operacao))
                {
                    ViewBag.Operacao = Operacao;
                    PreencheDadosParaView(Operacao, model);
                    return View(NomeViewForm, model);
                }

                // Executa a lógica de "pré-save" e obtém seu status
                bool beforeSaveComSucesso = ExecuteBeforeSave(model, Operacao);
                if (!beforeSaveComSucesso)
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

        /// <summary>
        /// Utilizado para implementar instruções que são executadas antes do método de Save
        /// </summary>
        /// <param name="model">dados da requisição</param>
        /// <param name="operacao">tipo de operação</param>
        /// <returns></returns>
        protected virtual bool ExecuteBeforeSave(T model, ModosOperacao operacao)
        {
            return true;
        }

        /// <summary>
        /// Implementa regras de validação da requisição
        /// </summary>
        /// <param name="model">dados para validar</param>
        /// <param name="operacao">tipo de operação</param>
        /// <returns></returns>
        protected virtual bool ValidaDados(T model, ModosOperacao operacao)
        {
            ModelState.Clear(); // inicia limpando o ModelState

            // Valida o gerenciamento de IDs dos CRUDs
            if (operacao == ModosOperacao.INCLUSAO && DAO.ObtemPorID(model.Id) != null)
                ModelState.AddModelError("Id", "Código já está em uso!");
            if (operacao == ModosOperacao.ALTERACAO && DAO.ObtemPorID(model.Id) == null)
                ModelState.AddModelError("Id", "Este registro não existe!");
            if (model.Id <= 0)
                ModelState.AddModelError("Id", "Id inválido!");

            return ModelState.IsValid;
        }

        /// <summary>
        /// Implementa a lógica base para edição de registros
        /// </summary>
        /// <param name="id">ID do registro editado</param>
        /// <returns></returns>
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

        /// <summary>
        /// Implementa lógica base para exibir os dados de um registro em tela
        /// </summary>
        /// <param name="id">id do registro</param>
        /// <returns></returns>
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

        /// <summary>
        /// Implementa a lógica base para exclusão de registros
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Implementa regra para buscar registros com base em filtros especificados em tela
        /// </summary>
        /// <param name="filtros">Model com os campos que devem ser filtrados no banco de dados</param>
        /// <returns></returns>
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
