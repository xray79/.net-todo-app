import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface Todo {
  id: number;
  title: string;
  isDone: boolean;
}

@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly http = inject(HttpClient);

  todos: Todo[] = [];
  newTodoTitle = '';
  loading = false;
  error = '';

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.loading = true;
    this.http.get<Todo[]>('http://localhost:5080/api/todos').subscribe({
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

    this.http.post<Todo>('http://localhost:5080/api/todos', { title }).subscribe({
      next: (todo) => {
        this.todos = [...this.todos, todo];
        this.newTodoTitle = '';
      },
      error: () => {
        this.error = 'Failed to add todo.';
      }
    });
  }

  toggleTodo(todo: Todo): void {
    this.http.put<Todo>(`http://localhost:5080/api/todos/${todo.id}`, {
      title: todo.title,
      isDone: !todo.isDone
    }).subscribe({
      next: (updated) => {
        this.todos = this.todos.map((item) => item.id === updated.id ? updated : item);
      },
      error: () => {
        this.error = 'Failed to update todo.';
      }
    });
  }

  deleteTodo(id: number): void {
    this.http.delete(`http://localhost:5080/api/todos/${id}`).subscribe({
      next: () => {
        this.todos = this.todos.filter((todo) => todo.id !== id);
      },
      error: () => {
        this.error = 'Failed to delete todo.';
      }
    });
  }
}
