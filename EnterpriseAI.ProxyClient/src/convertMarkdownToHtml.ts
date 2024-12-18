import Markdoc from "@markdoc/markdoc";

export function convertMarkdownToHtml(markdown: string) {
  const ast = Markdoc.parse(markdown);
  const content = Markdoc.transform(ast /* config */);

  const html = Markdoc.renderers.html(content);
  return html;
}
