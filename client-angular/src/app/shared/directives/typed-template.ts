import { Directive, Input } from '@angular/core';

@Directive({
  selector: '[typed]',
  standalone: true,
})
export class TypedTemplate<T> {
  @Input('typed') data!: T[];

  static ngTemplateContextGuard<T>(
    dir: TypedTemplate<T>,
    ctx: any
  ): ctx is { $implicit: T; index: number } {
    return true;
  }
}
