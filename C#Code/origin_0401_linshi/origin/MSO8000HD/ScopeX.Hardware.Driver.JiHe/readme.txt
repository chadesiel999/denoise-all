可能使用的条件编译包括：
Product_B21_JinHui_PXI
JiHe_MSO7000X
FrequencyDomain
DBI

1、在合并代码时，可能由于FPGA版本的原因，造成寄存器的不同，为了合并代码后工程编译通过，在合并是使用如下注释注释掉代码。
   合并后的代码分发后，实际的工程应该打开注释掉的代码。可以使用全工程范围的搜索，打开注释掉的代码外，也删除这个注释标签。
//commented by zhaoyong ,need opened at actual code
//end commented by zhaoyong ,need opened at actual code