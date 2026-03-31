# -*- coding: utf-8 -*-
import sys

path = r"c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"

replacements = [
    ("BÃƒÂ¡sica", "Básica"),
    ("ReferÃƒÂªncia", "Referência"),
    ("GÃƒÅ NEROS", "GÊNEROS"),
    ("HORÃƒÂ RIO", "HORÁRIO"),
    ("VOCÃƒÅ½", "VOCÊ"),
    ("VOCÃƒÅ ", "VOCÊ"),
    ("CALENDÃƒÂ RIO", "CALENDÁRIO"),
    ("TRÃƒÂ S", "TRÁS"),
    ("SeÃƒÂ§ÃƒÂ£o", "Seção"),
    ("SeÃƒÂ§ÃƒÂ£o", "Seção"), # Duplicate common mangling
    ("sÃƒÂ©rie", "série"),
    ("LÃƒâ€œGICA", "LÓGICA"),
    ("PontuaÃƒÂ§ÃƒÂ£o", "Pontuação"),
    ("GÃƒÂªneros", "Gêneros"),
    ("gÃƒÂªneros", "gêneros"),
    ("consistÃƒÂªncia", "consistência"),
    ("InfluÃƒÂªncias", "Influências"),
    ("sÃƒÂ£o", "são"),
    ("responsÃƒÂ¡veis", "responsáveis"),
    ("CabeÃƒÂ§alho", "Cabeçalho"),
    ("MÃƒÂªs", "Mês"),
    ("estÃƒÂ¡", "está"),
    ("TÃƒÂ­tulo", "Título"),
    ("InsÃƒÂ­gnia", "Insígnia"),
    ("informaÃƒÂ§ÃƒÂµes", "informações"),
    ("apresentarÃƒÂ¡", "apresentará"),
    ("DicionÃƒÂ¡rio", "Dicionário"),
    ("SESSÃƒâ€¢ES", "SESSÕES"),
    ("PERÃƒÂ ODO", "PERÍODO"),
    ("HISTÃƒâ€œRICO", "HISTÓRICO"),
    ("SESSÃƒÆ’O", "SESSÃO"),
    ("CONCLUÃƒ DO", "CONCLUÍDO"),
    ("Ã¢â‚¬â€", "—"),
    ("BotÃƒÂ£o", "Botão"),
    ("GrÃƒÂ¡fico", "Gráfico"),
    ("SessÃƒÂµes", "Sessões"),
]

with open(path, "rb") as f:
    content = f.read()

for target, replacement in replacements:
    content = content.replace(target.encode("utf-8"), replacement.encode("utf-8"))

with open(path, "wb") as f:
    f.write(content)

print("Encoding fix completed successfully.")
