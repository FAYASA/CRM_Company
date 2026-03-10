using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace seashore_CRM.BLL.DTOs
{
    // ================================
    // CREATE DTO
    // ================================
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }

    // ================================
    // UPDATE DTO
    // ================================
    public class CategoryUpdateDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = null!;

        public bool IsActive { get; set; }

        public byte[]? RowVersion { get; set; } // for concurrency
    }

    // ================================
    // LIST DTO (Index Page)
    // ================================
    public class CategoryListDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    // ================================
    // DETAIL DTO
    // ================================
    public class CategoryDetailDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public byte[]? RowVersion { get; set; }
    }
}
