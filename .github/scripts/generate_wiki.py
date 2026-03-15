#!/usr/bin/env python3
"""Gera documentação da Wiki do GitHub a partir do código-fonte C#.

Este script lê os arquivos .cs do projeto e gera páginas Markdown navegáveis
para a Wiki do GitHub, documentando a infraestrutura e as funcionalidades.
"""

import re
import sys
from pathlib import Path

SRC_ROOT = Path("src/ClaudeDotNetPlayground")
WIKI_OUTPUT = Path("wiki_output")

# ---------------------------------------------------------------------------
# Leitura de arquivos
# ---------------------------------------------------------------------------

def read_file(path: Path) -> str:
    try:
        return path.read_text(encoding="utf-8")
    except Exception:
        return ""


# ---------------------------------------------------------------------------
# Análise de código-fonte C#
# ---------------------------------------------------------------------------

def extract_class_name(content: str) -> str | None:
    match = re.search(
        r"public\s+(?:sealed\s+)?(?:static\s+)?(?:class|record|interface)\s+(\w+)",
        content,
    )
    return match.group(1) if match else None


def extract_route(content: str) -> str | None:
    match = re.search(r'\[Route\("([^"]+)"\)\]', content)
    return match.group(1) if match else None


def extract_http_methods(content: str) -> list[str]:
    methods = []
    for method in ["Get", "Post", "Put", "Delete", "Patch"]:
        if re.search(rf"\[Http{method}(?:\]|\()", content):
            methods.append(method.upper())
    return methods


def has_authenticate(content: str) -> bool:
    return "[Authenticate]" in content


def extract_public_methods(content: str) -> list[tuple[str, str]]:
    matches = re.findall(
        r"public\s+(?:async\s+)?(?:[\w<>?\[\]]+)\s+(\w+)\s*\(([^)]*)\)",
        content,
    )
    return [(name, params) for name, params in matches if name not in ("get", "set")]


def extract_models(content: str) -> list[dict]:
    """Extrai modelos (classes/records públicos) de um arquivo C#."""
    models = []
    pattern = re.compile(
        r"public\s+sealed\s+(?:class|record)\s+(\w+)"
        r"(?:\(([^)]*)\))?"  # primary constructor params
        r"(?:\s*\{([^{}]*(?:\{[^{}]*\}[^{}]*)*))?",  # class body
        re.DOTALL,
    )
    for m in pattern.finditer(content):
        name = m.group(1)
        record_params = m.group(2)
        class_body = m.group(3)

        properties = []
        if record_params:
            for param in record_params.split(","):
                param = param.strip()
                if not param:
                    continue
                parts = param.split()
                if len(parts) >= 2:
                    prop_type = parts[-2].rstrip("?")
                    prop_name = parts[-1]
                    properties.append((prop_type, prop_name))
        elif class_body:
            prop_matches = re.findall(
                r"public\s+([\w<>?\[\]]+)\s+(\w+)\s*\{", class_body
            )
            properties = [(t.rstrip("?"), n) for t, n in prop_matches]

        models.append({"name": name, "properties": properties})
    return models


# ---------------------------------------------------------------------------
# Componentes de infraestrutura
# ---------------------------------------------------------------------------

INFRA_DESCRIPTIONS: dict[str, str] = {
    "ITokenService": (
        "Contrato (interface) para geração e validação de tokens JWT. "
        "Define os métodos `GenerateToken` e `ValidateToken`."
    ),
    "TokenService": (
        "Implementação do serviço de token JWT usando o algoritmo HS256. "
        "Os tokens expiram em 1 hora e contêm os claims `id` e `userName`. "
        "A chave de assinatura é lida da configuração `Jwt:Secret`."
    ),
    "AuthenticatedUser": (
        "Modelo que representa o usuário autenticado extraído do token JWT, "
        "contendo as propriedades `Id` (int) e `UserName` (string)."
    ),
    "AuthenticateFilter": (
        "Filtro de ação assíncrono (`IAsyncActionFilter`) que valida o Bearer Token "
        "em cada requisição protegida. Quando o token é válido, enriquece os logs do "
        "Serilog com as propriedades `UserId` e `UserName`. Retorna HTTP 401 com "
        "Problem Details se o token estiver ausente ou inválido."
    ),
    "AuthenticateAttribute": (
        "Atributo decorador (`TypeFilterAttribute`) aplicado nos controllers para "
        "ativar o `AuthenticateFilter` via injeção de dependência. Uso: `[Authenticate]`."
    ),
    "CorrelationIdMiddleware": (
        "Middleware que garante um identificador único (GUID v7) por requisição. "
        "Lê o header `X-Correlation-Id` de entrada (se for um GUID v7 válido) ou "
        "gera um novo. Propaga o valor no header de resposta e enriquece os logs do "
        "Serilog com a propriedade `CorrelationId`. Completamente transparente para as features."
    ),
    "GlobalExceptionHandler": (
        "Handler centralizado de exceções não tratadas (`IExceptionHandler`). "
        "Intercepta qualquer exceção que escapa do pipeline, registra o erro nos logs "
        "e retorna uma resposta HTTP 500 no formato Problem Details (RFC 7807)."
    ),
    "GuidV7": (
        "Utilitário interno para criação e validação de GUIDs na versão 7 (time-ordered). "
        "Utilizado pelo `CorrelationIdMiddleware` para garantir identificadores "
        "monotônicos, rastreáveis e compatíveis com a especificação RFC 9562."
    ),
}

INFRA_GROUP_NAMES_PT: dict[str, str] = {
    "Security": "Segurança",
    "Middlewares": "Middlewares",
    "ExceptionHandling": "Tratamento de Exceções",
    "Correlation": "Correlação",
}


def describe_component(name: str) -> str:
    return INFRA_DESCRIPTIONS.get(name, f"Componente `{name}` da camada de infraestrutura.")


def extract_infra_components(infra_path: Path) -> dict:
    groups: dict[str, dict] = {}
    for group_dir in sorted(infra_path.iterdir()):
        if not group_dir.is_dir():
            continue
        group_name = group_dir.name
        components = []
        for cs_file in sorted(group_dir.glob("*.cs")):
            content = read_file(cs_file)
            class_name = extract_class_name(content) or cs_file.stem
            methods = extract_public_methods(content)
            components.append(
                {
                    "name": class_name,
                    "file": cs_file.name,
                    "methods": methods,
                    "content": content,
                }
            )
        if components:
            groups[group_name] = {
                "name_pt": INFRA_GROUP_NAMES_PT.get(group_name, group_name),
                "components": components,
            }
    return groups


# ---------------------------------------------------------------------------
# Descoberta de features
# ---------------------------------------------------------------------------

FEATURE_DESCRIPTIONS: dict[str, str] = {
    "TestGet": (
        "Endpoint de verificação de disponibilidade da aplicação. "
        "Retorna a string `\"funcionando\"` para confirmar que o serviço está em operação."
    ),
    "UserLogin": (
        "Endpoint de autenticação de usuários. Valida as credenciais fornecidas "
        "(`UserName` e `Password`) e, em caso de sucesso, retorna um Bearer Token JWT "
        "para ser usado nas demais requisições protegidas."
    ),
}


def describe_feature(feature: dict) -> str:
    return FEATURE_DESCRIPTIONS.get(
        feature["name"],
        f"Funcionalidade `{feature['name']}` do tipo {feature['type']}.",
    )


def get_features(features_path: Path) -> list[dict]:
    features = []
    for category in ["Query", "Command"]:
        cat_path = features_path / category
        if not cat_path.exists():
            continue
        for feature_dir in sorted(cat_path.iterdir()):
            if not feature_dir.is_dir():
                continue
            feature: dict = {
                "name": feature_dir.name,
                "type": category,
                "path": feature_dir,
                "route": None,
                "methods": [],
                "authenticated": False,
                "models": [],
            }

            # Endpoint
            for ep_dir in feature_dir.glob("*Endpoint"):
                if ep_dir.is_dir():
                    for cs_file in ep_dir.glob("*.cs"):
                        content = read_file(cs_file)
                        feature["route"] = extract_route(content)
                        feature["methods"] = extract_http_methods(content)
                        feature["authenticated"] = has_authenticate(content)
                    break

            # Models
            for model_dir in feature_dir.glob("*Models"):
                if model_dir.is_dir():
                    for cs_file in model_dir.glob("*.cs"):
                        feature["models"] = extract_models(read_file(cs_file))
                    break

            features.append(feature)
    return features


# ---------------------------------------------------------------------------
# Navegação
# ---------------------------------------------------------------------------

def nav_bar(
    prev_page: tuple[str, str] | None,
    next_page: tuple[str, str] | None,
) -> str:
    parts = []
    if prev_page:
        label, link = prev_page
        parts.append(f"[← {label}]({link})")
    parts.append("[🏠 Início](Home)")
    if next_page:
        label, link = next_page
        parts.append(f"[{label} →]({link})")
    return " | ".join(parts)


def format_endpoint(feature: dict) -> str:
    methods = feature["methods"]
    route = feature["route"] or "?"
    if methods:
        return f"{', '.join(methods)} /{route}"
    return f"/{route}"


def get_type_pt(feature_type: str) -> str:
    return "Query (Leitura)" if feature_type == "Query" else "Command (Escrita)"


# ---------------------------------------------------------------------------
# Geração das páginas
# ---------------------------------------------------------------------------

def generate_home(features: list[dict], infra_groups: dict) -> str:
    lines = []
    lines.append("# ClaudeDotNetPlayground\n")
    lines.append(
        "Projeto de playground em .NET 10 utilizando ASP.NET Core com arquitetura Vertical Slice. "
        "As funcionalidades são organizadas em operações de leitura (**Query**) e escrita (**Command**), "
        "com uma camada de infraestrutura transversal responsável por segurança, rastreabilidade e tratamento de erros.\n"
    )
    lines.append("## Seções\n")
    lines.append("| Seção | Descrição |")
    lines.append("|---|---|")
    lines.append(
        "| [Infraestrutura](Infrastructure) | Componentes transversais: middleware, segurança e tratamento de exceções |"
    )
    lines.append(
        "| [Funcionalidades](Features) | Funcionalidades organizadas por Query (leitura) e Command (escrita) |\n"
    )
    lines.append("---\n")

    # Infrastructure summary
    lines.append("## Infraestrutura\n")
    lines.append(
        "A camada de infraestrutura fornece recursos compartilhados por todas as funcionalidades:\n"
    )
    for group_name, group in infra_groups.items():
        comp_names = ", ".join(f"`{c['name']}`" for c in group["components"])
        lines.append(f"- **{group['name_pt']}**: {comp_names}")
    lines.append("")
    lines.append("→ [Ver documentação completa da Infraestrutura](Infrastructure)\n")
    lines.append("---\n")

    # Features summary
    lines.append("## Funcionalidades\n")
    query_features = [f for f in features if f["type"] == "Query"]
    command_features = [f for f in features if f["type"] == "Command"]

    if query_features:
        lines.append("### Funcionalidades de Consulta (Query)\n")
        lines.append("| Funcionalidade | Endpoint | Descrição |")
        lines.append("|---|---|---|")
        for f in query_features:
            page = f"Feature-Query-{f['name']}"
            endpoint = format_endpoint(f)
            desc = describe_feature(f)
            lines.append(f"| [{f['name']}]({page}) | `{endpoint}` | {desc} |")
        lines.append("")

    if command_features:
        lines.append("### Funcionalidades de Comando (Command)\n")
        lines.append("| Funcionalidade | Endpoint | Descrição |")
        lines.append("|---|---|---|")
        for f in command_features:
            page = f"Feature-Command-{f['name']}"
            endpoint = format_endpoint(f)
            desc = describe_feature(f)
            lines.append(f"| [{f['name']}]({page}) | `{endpoint}` | {desc} |")
        lines.append("")

    lines.append("→ [Ver documentação completa das Funcionalidades](Features)\n")
    return "\n".join(lines)


def generate_infrastructure(
    infra_groups: dict,
    prev_page: tuple[str, str] | None,
    next_page: tuple[str, str] | None,
) -> str:
    nav = nav_bar(prev_page, next_page)
    lines = []
    lines.append(nav + "\n")
    lines.append("---\n")
    lines.append("# Infraestrutura\n")
    lines.append(
        "A camada de infraestrutura (`src/ClaudeDotNetPlayground/Infra/`) fornece componentes "
        "transversais utilizados por todas as funcionalidades. Nenhuma feature depende diretamente "
        "desta camada — os componentes são injetados automaticamente pelo pipeline da aplicação.\n"
    )

    for group_name, group in infra_groups.items():
        lines.append("---\n")
        lines.append(f"## {group['name_pt']}\n")
        for comp in group["components"]:
            lines.append(f"### `{comp['name']}`\n")
            lines.append(describe_component(comp["name"]) + "\n")
            visible_methods = [
                (n, p) for n, p in comp["methods"]
                if not n[0].islower() or n in ("get", "set")
            ]
            if visible_methods:
                lines.append("**Métodos públicos:**\n")
                for method_name, method_params in visible_methods:
                    lines.append(f"- `{method_name}({method_params.strip()})`")
                lines.append("")

    lines.append("---\n")
    lines.append(nav + "\n")
    return "\n".join(lines)


def generate_features_overview(
    features: list[dict],
    prev_page: tuple[str, str] | None,
    next_page: tuple[str, str] | None,
) -> str:
    nav = nav_bar(prev_page, next_page)
    lines = []
    lines.append(nav + "\n")
    lines.append("---\n")
    lines.append("# Funcionalidades\n")
    lines.append(
        "As funcionalidades são implementadas seguindo a arquitetura Vertical Slice, onde cada "
        "funcionalidade é uma fatia vertical independente contendo todos os seus artefatos: "
        "endpoint, caso de uso, modelos e interfaces.\n"
    )

    query_features = [f for f in features if f["type"] == "Query"]
    command_features = [f for f in features if f["type"] == "Command"]

    if query_features:
        lines.append("## Funcionalidades de Consulta (Query — Leitura)\n")
        lines.append("Operações que **não** alteram o estado do sistema.\n")
        lines.append("| Funcionalidade | Endpoint | Autenticação | Descrição |")
        lines.append("|---|---|---|---|")
        for f in query_features:
            page = f"Feature-Query-{f['name']}"
            endpoint = format_endpoint(f)
            auth = "Sim" if f["authenticated"] else "Não"
            desc = describe_feature(f)
            lines.append(f"| [{f['name']}]({page}) | `{endpoint}` | {auth} | {desc} |")
        lines.append("")

    if command_features:
        lines.append("## Funcionalidades de Comando (Command — Escrita)\n")
        lines.append("Operações que **alteram** o estado do sistema.\n")
        lines.append("| Funcionalidade | Endpoint | Autenticação | Descrição |")
        lines.append("|---|---|---|---|")
        for f in command_features:
            page = f"Feature-Command-{f['name']}"
            endpoint = format_endpoint(f)
            auth = "Sim" if f["authenticated"] else "Não"
            desc = describe_feature(f)
            lines.append(f"| [{f['name']}]({page}) | `{endpoint}` | {auth} | {desc} |")
        lines.append("")

    lines.append("---\n")
    lines.append(nav + "\n")
    return "\n".join(lines)


def generate_feature_page(
    feature: dict,
    prev_page: tuple[str, str] | None,
    next_page: tuple[str, str] | None,
) -> str:
    nav = nav_bar(prev_page, next_page)
    lines = []
    lines.append(nav + "\n")
    lines.append("---\n")
    lines.append(f"# {feature['name']}\n")
    lines.append(f"**Tipo**: {get_type_pt(feature['type'])}")
    lines.append(f"**Endpoint**: `{format_endpoint(feature)}`")
    auth_text = (
        "Obrigatória — Bearer Token JWT"
        if feature["authenticated"]
        else "Não obrigatória"
    )
    lines.append(f"**Autenticação**: {auth_text}\n")

    lines.append("## O Que Faz\n")
    lines.append(describe_feature(feature) + "\n")

    # Request
    lines.append("## Requisição\n")
    input_models = [m for m in feature["models"] if "Input" in m["name"]]
    if input_models and input_models[0]["properties"]:
        model = input_models[0]
        lines.append("**Corpo da requisição (JSON):**\n")
        lines.append("| Campo | Tipo | Obrigatório |")
        lines.append("|---|---|---|")
        for prop_type, prop_name in model["properties"]:
            lines.append(f"| `{prop_name}` | `{prop_type}` | Sim |")
        lines.append("")
    else:
        lines.append("Sem corpo de requisição.\n")

    # Response
    lines.append("## Resposta\n")
    output_models = [m for m in feature["models"] if "Output" in m["name"]]
    lines.append("| Status HTTP | Descrição |")
    lines.append("|---|---|")

    if output_models and output_models[0]["properties"]:
        model = output_models[0]
        fields = ", ".join(
            f"`{name}`: `{ptype}`" for ptype, name in model["properties"]
        )
        lines.append(f"| `200 OK` | Objeto com {fields} |")
    elif output_models:
        lines.append(f"| `200 OK` | `{output_models[0]['name']}` |")
    else:
        lines.append('| `200 OK` | `"funcionando"` |')

    if feature["authenticated"]:
        lines.append(
            "| `401 Não Autorizado` | Token ausente ou inválido — retorna Problem Details (RFC 7807) |"
        )
    else:
        lines.append(
            "| `401 Não Autorizado` | Credenciais inválidas — retorna Problem Details (RFC 7807) |"
        )

    lines.append("")
    lines.append("---\n")
    lines.append(nav + "\n")
    return "\n".join(lines)


# ---------------------------------------------------------------------------
# Ponto de entrada
# ---------------------------------------------------------------------------

def main() -> None:
    if not SRC_ROOT.exists():
        print(f"Erro: diretório '{SRC_ROOT}' não encontrado.", file=sys.stderr)
        sys.exit(1)

    WIKI_OUTPUT.mkdir(exist_ok=True)

    infra_groups = extract_infra_components(SRC_ROOT / "Infra")
    features = get_features(SRC_ROOT / "Features")

    query_features = [f for f in features if f["type"] == "Query"]
    command_features = [f for f in features if f["type"] == "Command"]
    all_features = query_features + command_features

    # Home
    (WIKI_OUTPUT / "Home.md").write_text(
        generate_home(features, infra_groups), encoding="utf-8"
    )
    print("✓ Home.md gerado")

    # Infrastructure
    infra_next: tuple[str, str] | None = (
        ("Funcionalidades", "Features") if all_features else None
    )
    (WIKI_OUTPUT / "Infrastructure.md").write_text(
        generate_infrastructure(infra_groups, None, infra_next), encoding="utf-8"
    )
    print("✓ Infrastructure.md gerado")

    # Features overview
    feat_prev: tuple[str, str] = ("Infraestrutura", "Infrastructure")
    feat_next: tuple[str, str] | None = (
        (all_features[0]["name"], f"Feature-{all_features[0]['type']}-{all_features[0]['name']}")
        if all_features
        else None
    )
    (WIKI_OUTPUT / "Features.md").write_text(
        generate_features_overview(features, feat_prev, feat_next), encoding="utf-8"
    )
    print("✓ Features.md gerado")

    # Individual feature pages
    for i, feature in enumerate(all_features):
        page_name = f"Feature-{feature['type']}-{feature['name']}"

        prev_feature_page: tuple[str, str]
        if i == 0:
            prev_feature_page = ("Funcionalidades", "Features")
        else:
            pf = all_features[i - 1]
            prev_feature_page = (pf["name"], f"Feature-{pf['type']}-{pf['name']}")

        next_feature_page: tuple[str, str] | None = None
        if i < len(all_features) - 1:
            nf = all_features[i + 1]
            next_feature_page = (nf["name"], f"Feature-{nf['type']}-{nf['name']}")

        (WIKI_OUTPUT / f"{page_name}.md").write_text(
            generate_feature_page(feature, prev_feature_page, next_feature_page),
            encoding="utf-8",
        )
        print(f"✓ {page_name}.md gerado")

    total = len(list(WIKI_OUTPUT.glob("*.md")))
    print(f"\n✅ Wiki gerada em '{WIKI_OUTPUT}/' com {total} páginas.")


if __name__ == "__main__":
    main()
