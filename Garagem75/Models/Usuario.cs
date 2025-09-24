﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Garagem75.Models;

[Table("Usuario")]
public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "Campo obrigatório!")]
    [Display(Name = "Nome")]
    [StringLength(75)]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Campo obrigatório!")]
    [Display(Name = "Email")]
    [StringLength(75)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Campo obrigatório!")]
    [Display(Name = "Senha")]
    [StringLength(6)]
    public string Senha { get; set; }

    [Required(ErrorMessage = "Campo obrigatório!")]
    [Display(Name = "Tipo de Usuário")]
    public int TipoUsuarioId { get; set; }
    public virtual TipoUsuario? TipoUsuario { get; set; }

    //Propriedade para Softdelete
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;


}

