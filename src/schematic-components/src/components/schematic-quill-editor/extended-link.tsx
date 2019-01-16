import Quill from 'quill';
const Link = Quill.import('formats/link');

export default class ExtendedLink extends Link {
  private static sanitizeUrl(url: string, protocols: any[]) {
    const anchor = document.createElement('a');
    anchor.href = url;
    const protocol = anchor.href.slice(0, anchor.href.indexOf(':'));
    return protocols.indexOf(protocol) > -1;
  }

  static create(value: string) {
    const node = super.create(value);
    node.setAttribute('href', this.sanitize(value));

    for (const protocol of ExtendedLink.PROTOCOL_WHITELIST) {
      if (value.startsWith(protocol)) {
        node.setAttribute('rel', 'noopener');
        node.setAttribute('target', '_blank');
        break;
      }
    }

    return node;
  }

  static formats(domNode: HTMLElement) {
    return domNode.getAttribute('href');
  }

  static sanitize(url: string) {
    return this.sanitizeUrl(url, this.PROTOCOL_WHITELIST) ? url : this.SANITIZED_URL;
  }
}

ExtendedLink.blotName = 'link';
ExtendedLink.tagName = 'A';
ExtendedLink.SANITIZED_URL = 'about:blank';
ExtendedLink.PROTOCOL_WHITELIST = ['http', 'https', 'mailto', 'tel'];