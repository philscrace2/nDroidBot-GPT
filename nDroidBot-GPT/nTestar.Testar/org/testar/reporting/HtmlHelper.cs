using System.Text;

namespace org.testar.reporting
{
    public static class HtmlHelper
    {
        public static string getHtmlScript()
        {
            var scriptJoiner = new StringBuilder();

            scriptJoiner.AppendLine("// Feature to reverse state and action div nodes");
            scriptJoiner.AppendLine("function reverse(){");
            scriptJoiner.AppendLine("let direction = document.getElementById('main').style.flexDirection;");
            scriptJoiner.AppendLine("if(direction === 'column') document.getElementById('main').style.flexDirection = 'column-reverse';");
            scriptJoiner.AppendLine("else document.getElementById('main').style.flexDirection = 'column';}");
            scriptJoiner.AppendLine("");

            scriptJoiner.AppendLine("// Feature to enable derived action nodes to be shown or hidden");
            scriptJoiner.AppendLine("function toggleCollapsible(){");
            scriptJoiner.AppendLine("document.querySelectorAll('.collapsible').forEach(btn => {");
            scriptJoiner.AppendLine("btn.addEventListener('click', function() {");
            scriptJoiner.AppendLine("this.classList.toggle('active');");
            scriptJoiner.AppendLine("let content = this.nextElementSibling;");
            scriptJoiner.AppendLine("if (content.style.maxHeight) {");
            scriptJoiner.AppendLine("content.style.maxHeight = null;");
            scriptJoiner.AppendLine("} else {");
            scriptJoiner.AppendLine("content.style.maxHeight = content.scrollHeight + 'px';");
            scriptJoiner.AppendLine("}");
            scriptJoiner.AppendLine("}); }); }");
            scriptJoiner.AppendLine("document.addEventListener('DOMContentLoaded', toggleCollapsible);");
            scriptJoiner.AppendLine("");

            scriptJoiner.AppendLine("// Feature to highlight the visualizer of the verdict widget");
            scriptJoiner.AppendLine("document.addEventListener('DOMContentLoaded', function () {");
            scriptJoiner.AppendLine("const visualizerText = document.getElementById('visualizer-rect')?.innerText;");
            scriptJoiner.AppendLine("if (!visualizerText) return;");
            scriptJoiner.AppendLine("const rectPattern = /Rect\\s*\\[x:(\\d+\\.\\d+)\\s*y:(\\d+\\.\\d+)\\s*w:(\\d+\\.\\d+)\\s*h:(\\d+\\.\\d+)\\]/g;");
            scriptJoiner.AppendLine("const matches = [...visualizerText.matchAll(rectPattern)];");
            scriptJoiner.AppendLine("if (matches.length === 0) return;");
            scriptJoiner.AppendLine("const visualizerElement = document.getElementById('visualizer-rect');");
            scriptJoiner.AppendLine("let stateBlock = visualizerElement.closest('.stateBlock');");
            scriptJoiner.AppendLine("if (!stateBlock) {");
            scriptJoiner.AppendLine("let sibling = visualizerElement.parentElement?.previousElementSibling;");
            scriptJoiner.AppendLine("while (sibling && !sibling.classList.contains('stateBlock')) {");
            scriptJoiner.AppendLine("sibling = sibling.previousElementSibling;");
            scriptJoiner.AppendLine("}");
            scriptJoiner.AppendLine("stateBlock = sibling;");
            scriptJoiner.AppendLine("}");
            scriptJoiner.AppendLine("const imgContainer = stateBlock.querySelector('.background');");
            scriptJoiner.AppendLine("const img = imgContainer?.querySelector('img');");
            scriptJoiner.AppendLine("if (!img) return;");
            scriptJoiner.AppendLine("function updateRectangles() {");
            scriptJoiner.AppendLine("const imgRect = img.getBoundingClientRect();");
            scriptJoiner.AppendLine("const containerRect = imgContainer.getBoundingClientRect();");
            scriptJoiner.AppendLine("const offsetX = imgRect.left - containerRect.left;");
            scriptJoiner.AppendLine("const offsetY = imgRect.top - containerRect.top;");
            scriptJoiner.AppendLine("const scaleX = imgRect.width / img.naturalWidth;");
            scriptJoiner.AppendLine("const scaleY = imgRect.height / img.naturalHeight;");
            scriptJoiner.AppendLine("const existingRectangles = imgContainer.querySelectorAll('.rectangle');");
            scriptJoiner.AppendLine("existingRectangles.forEach(rect => rect.remove());");
            scriptJoiner.AppendLine("matches.forEach(match => {");
            scriptJoiner.AppendLine("const [x, y, width, height] = match.slice(1, 5).map(parseFloat);");
            scriptJoiner.AppendLine("if (x > img.naturalWidth || y > img.naturalHeight || (x + width) < 0 || (y + height) < 0) return;");
            scriptJoiner.AppendLine("if (width === 0 && height === 0) return;");
            scriptJoiner.AppendLine("const rectangleDiv = document.createElement('div');");
            scriptJoiner.AppendLine("rectangleDiv.classList.add('rectangle');");
            scriptJoiner.AppendLine("Object.assign(rectangleDiv.style, {");
            scriptJoiner.AppendLine("position: 'absolute',");
            scriptJoiner.AppendLine("left: (x * scaleX + offsetX) - 3 + 'px',");
            scriptJoiner.AppendLine("top: (y * scaleY + offsetY) - 3 + 'px',");
            scriptJoiner.AppendLine("width: (width * scaleX) - 6 + 'px',");
            scriptJoiner.AppendLine("height: (height * scaleY) - 6 + 'px',");
            scriptJoiner.AppendLine("border: '2px solid red'");
            scriptJoiner.AppendLine("});");
            scriptJoiner.AppendLine("imgContainer.appendChild(rectangleDiv);");
            scriptJoiner.AppendLine("});");
            scriptJoiner.AppendLine("}");
            scriptJoiner.AppendLine("img.onload = updateRectangles;");
            scriptJoiner.AppendLine("window.addEventListener('resize', updateRectangles);");
            scriptJoiner.AppendLine("});");

            return scriptJoiner.ToString();
        }

        public static string getHtmlStyle()
        {
            var styleJoiner = new StringBuilder();

            styleJoiner.AppendLine("div {");
            styleJoiner.AppendLine("border: 1px solid black;");
            styleJoiner.AppendLine("margin: 2px;");
            styleJoiner.AppendLine("padding: 2px;");
            styleJoiner.AppendLine("}");

            styleJoiner.AppendLine(".background {");
            styleJoiner.AppendLine("padding: 15px;");
            styleJoiner.AppendLine("width: 100%;");
            styleJoiner.AppendLine("border: none;");
            styleJoiner.AppendLine("text-align: left;");
            styleJoiner.AppendLine("outline: none;");
            styleJoiner.AppendLine("border-radius: 4px;");
            styleJoiner.AppendLine("position: relative;");
            styleJoiner.AppendLine("}");

            styleJoiner.AppendLine(".background img {");
            styleJoiner.AppendLine("max-width: 98%;");
            styleJoiner.AppendLine("max-height: 98%;");
            styleJoiner.AppendLine("object-fit: contain;");
            styleJoiner.AppendLine("}");

            styleJoiner.AppendLine(".collapsible {");
            styleJoiner.AppendLine("color: black;");
            styleJoiner.AppendLine("font-weight: bold;");
            styleJoiner.AppendLine("cursor: pointer;");
            styleJoiner.AppendLine("padding: 15px;");
            styleJoiner.AppendLine("width: 100%;");
            styleJoiner.AppendLine("border: none;");
            styleJoiner.AppendLine("text-align: left;");
            styleJoiner.AppendLine("outline: none;");
            styleJoiner.AppendLine("font-size: 18px;");
            styleJoiner.AppendLine("border-radius: 4px;");
            styleJoiner.AppendLine("}");

            styleJoiner.AppendLine(".active, .collapsible:hover {}");

            styleJoiner.AppendLine(".collapsible:after {");
            styleJoiner.AppendLine("content: '\\25B6';");
            styleJoiner.AppendLine("color: black;");
            styleJoiner.AppendLine("font-weight: bold;");
            styleJoiner.AppendLine("float: left;");
            styleJoiner.AppendLine("margin-left: 2px;");
            styleJoiner.AppendLine("margin-right: 10px;");
            styleJoiner.AppendLine("}");

            styleJoiner.AppendLine(".collapsible.active:after {");
            styleJoiner.AppendLine("content: '\\25BC';");
            styleJoiner.AppendLine("}");

            styleJoiner.AppendLine(".collapsibleContent {");
            styleJoiner.AppendLine("padding: 0 15px;");
            styleJoiner.AppendLine("max-height: 0;");
            styleJoiner.AppendLine("overflow: hidden;");
            styleJoiner.AppendLine("border-radius: 4px;");
            styleJoiner.AppendLine("transition: max-height 0.3s ease-out;");
            styleJoiner.AppendLine("}");

            return styleJoiner.ToString();
        }
    }
}
