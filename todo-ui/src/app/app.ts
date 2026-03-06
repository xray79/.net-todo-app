import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface Todo {
  id: number;
  title: string;
  isDone: boolean;
}

interface AuthResponse {
  token: string;
  email: string;
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

  authMode: 'login' | 'register' = 'login';
  email = '';
  password = '';
  authError = '';
  isAuthenticated = false;

  ngOnInit(): void {
    const token = localStorage.getItem('todo_token');
    this.isAuthenticated = !!token;

    if (this.isAuthenticated) {
      this.loadTodos();
    }
  }

  switchMode(mode: 'login' | 'register'): void {
    this.authMode = mode;
    this.authError = '';
  }

  register(): void {
    this.authError = '';

    this.http.post<AuthResponse>('http://localhost:5080/api/auth/register', {
      email: this.email,
      password: this.password
    }).subscribe({
      next: (response) => this.completeAuth(response),
      error: () => {
        this.authError = 'Registration failed. Try a different email or stronger password.';
      }
    });
  }

  login(): void {
    this.authError = '';

    this.http.post<AuthResponse>('http://localhost:5080/api/auth/login', {
      email: this.email,
      password: this.password
    }).subscribe({
      next: (response) => this.completeAuth(response),
      error: () => {
        this.authError = 'Login failed. Check your email and password.';
      }
    });
  }

  logout(): void {
    localStorage.removeItem('todo_token');
    this.isAuthenticated = false;
    this.todos = [];
    this.newTodoTitle = '';
    this.error = '';
  }

  loadTodos(): void {
    this.loading = true;

    this.http.get<Todo[]>('http://localhost:5080/api/todos', {
      headers: this.authHeaders()
    }).subscribe({
      next: (todos) => {
        this.todos = todos;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load todos. Please log in again.';
        this.loading = false;
      }
    });
  }

  addTodo(): void {
    const title = this.newTodoTitle.trim();
    if (!title) {
      return;
    }

    this.http.post<Todo>('http://localhost:5080/api/todos', { title }, {
      headers: this.authHeaders()
    }).subscribe({
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
    }, {
      headers: this.authHeaders()
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
    this.http.delete(`http://localhost:5080/api/todos/${id}`, {
      headers: this.authHeaders()
    }).subscribe({
      next: () => {
        this.todos = this.todos.filter((todo) => todo.id !== id);
      },
      error: () => {
        this.error = 'Failed to delete todo.';
      }
    });
  }

  private completeAuth(response: AuthResponse): void {
    localStorage.setItem('todo_token', response.token);
    this.isAuthenticated = true;
    this.authError = '';
    this.error = '';
    this.loadTodos();
  }

  private authHeaders(): HttpHeaders {
    const token = localStorage.getItem('todo_token') ?? '';
    return new HttpHeaders({ Authorization: `Bearer ${token}` });
  }
}
