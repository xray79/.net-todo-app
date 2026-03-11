import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Todo {
  id: number;
  title: string;
  isDone: boolean;
}

@Injectable({ providedIn: 'root' })
export class TodoApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5080/api/todos';

  getTodos(): Observable<Todo[]> {
    return this.http.get<Todo[]>(this.baseUrl);
  }

  createTodo(title: string): Observable<Todo> {
    return this.http.post<Todo>(this.baseUrl, { title });
  }

  updateTodo(id: number, title: string, isDone: boolean): Observable<Todo> {
    return this.http.put<Todo>(`${this.baseUrl}/${id}`, { title, isDone });
  }

  completeTodo(id: number): Observable<Todo> {
    return this.http.patch<Todo>(`${this.baseUrl}/${id}/complete`, {});
  }

  deleteTodo(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
