import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Todo, TodoApiService } from './services/todo-api.service';

@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly todoApi = inject(TodoApiService);

  todos: Todo[] = [];
  newTodoTitle = '';
  loading = false;
  error = '';

  editingTodoId: number | null = null;
  editingTitle = '';

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.loading = true;
    this.todoApi.getTodos().subscribe({
      next: (todos) => {
        this.todos = todos;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load todos. Is the API running on port 5080?';
        this.loading = false;
      }
    });
  }

  addTodo(): void {
    const title = this.newTodoTitle.trim();
    if (!title) {
      return;
    }

    this.todoApi.createTodo(title).subscribe({
      next: (todo) => {
        this.todos = [...this.todos, todo];
        this.newTodoTitle = '';
      },
      error: () => {
        this.error = 'Failed to add todo.';
      }
    });
  }

  startEdit(todo: Todo): void {
    this.editingTodoId = todo.id;
    this.editingTitle = todo.title;
  }

  cancelEdit(): void {
    this.editingTodoId = null;
    this.editingTitle = '';
  }

  saveEdit(todo: Todo): void {
    const title = this.editingTitle.trim();
    if (!title) {
      return;
    }

    this.todoApi.updateTodo(todo.id, title, todo.isDone).subscribe({
      next: (updated) => {
        this.todos = this.todos.map((item) => item.id === updated.id ? updated : item);
        this.cancelEdit();
      },
      error: () => {
        this.error = 'Failed to update todo.';
      }
    });
  }

  toggleTodo(todo: Todo): void {
    if (!todo.isDone) {
      this.todoApi.completeTodo(todo.id).subscribe({
        next: (updated) => {
          this.todos = this.todos.map((item) => item.id === updated.id ? updated : item);
        },
        error: () => {
          this.error = 'Failed to complete todo.';
        }
      });

      return;
    }

    this.todoApi.updateTodo(todo.id, todo.title, false).subscribe({
      next: (updated) => {
        this.todos = this.todos.map((item) => item.id === updated.id ? updated : item);
      },
      error: () => {
        this.error = 'Failed to update todo.';
      }
    });
  }

  deleteTodo(id: number): void {
    this.todoApi.deleteTodo(id).subscribe({
      next: () => {
        this.todos = this.todos.filter((todo) => todo.id !== id);
      },
      error: () => {
        this.error = 'Failed to delete todo.';
      }
    });
  }
}
