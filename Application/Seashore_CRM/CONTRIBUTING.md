# .editorconfig
root = true
[*]
charset = utf-8
indent_style = space
indent_size = 4
insert_final_newline = true

# CONTRIBUTING.md
# Project conventions

## DTO mapping conventions
- `LeadProductDto.LeadProductId` must represent the LeadItem primary key (LeadItems.Id).
- `LeadProductDto.ProductId` must represent the referenced Product Id.

## Client binding
- HTML inputs must bind `Lead.ProductItems[n].LeadProductId` for existing row id and `Lead.ProductItems[n].ProductId` for selected product id.