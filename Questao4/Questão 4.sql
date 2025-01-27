SELECT 
    assunto, 
    ano, 
    COUNT(*) quantidade
FROM 
    atendimentos
GROUP BY 
    assunto, ano
HAVING 
    COUNT(*) > 3
ORDER BY 
    quantidade DESC,
    ano ASC; 
