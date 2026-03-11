import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { App } from './app';
import { TodoApiService } from './services/todo-api.service';

class TodoApiServiceStub {
  getTodos() {
    return of([]);
  }

  createTodo(title: string) {
    return of({ id: 1, title, isDone: false });
  }

  updateTodo(id: number, title: string, isDone: boolean) {
    return of({ id, title, isDone });
  }

  completeTodo(id: number) {
    return of({ id, title: 'done', isDone: true });
  }

  deleteTodo() {
    return of(undefined);
  }
}

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [{ provide: TodoApiService, useClass: TodoApiServiceStub }]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render title', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Todo App');
  });

  it('should switch into editing mode', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;

    app.startEdit({ id: 5, title: 'Edit me', isDone: false });

    expect(app.editingTodoId).toBe(5);
    expect(app.editingTitle).toBe('Edit me');
  });
});
