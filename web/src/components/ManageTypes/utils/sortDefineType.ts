import { DirectiveOptions } from 'vue';
import Sortable from 'sortablejs';

const sortDefineType: DirectiveOptions = {
  bind(el, binding, vnode) {
    const tableRows = el.querySelector('tbody');
    Sortable.create(tableRows, {
      animation: 50,
      handle: '.handle',
      chosenClass: 'is-selected',
      sort: true,
      onEnd: function(evt: any) {
        const ctx = vnode.context as any;
        if (!ctx || !ctx.typeList) return;
        const items = ctx.typeList;

        // Find the count of system items at the top
        const systemCount = items.filter((item: any) => item.isSystem).length;
        // Find the count of expired items at the bottom
        const expiredCount = items.filter((item: any) => item._rowVariant || item.expiryDate).length;
        const expiredStart = items.length - expiredCount;

        // Prevent moving any item before the first non-system item
        if (evt.newIndex < systemCount) {
          ctx.updateTable++;
          return;
        }

        // Prevent moving system items at all
        if (items[evt.oldIndex].isSystem) {
          ctx.updateTable++;
          return;
        }

        // Prevent moving expired items at all
        if (items[evt.oldIndex]._rowVariant || items[evt.oldIndex].expiryDate) {
          ctx.updateTable++;
          return;
        }

        // Prevent moving any item into the expired section
        if (evt.newIndex >= expiredStart) {
          ctx.updateTable++;
          return;
        }

        // Move only active, non-system items within their allowed range
        const movedItem = items.splice(evt.oldIndex, 1)[0];
        items.splice(evt.newIndex, 0, movedItem);
        ctx.updateTable++;
        if (typeof ctx.saveSortOrders === 'function') {
          ctx.saveSortOrders();
        }
      }
    });
  }
};

export default sortDefineType;