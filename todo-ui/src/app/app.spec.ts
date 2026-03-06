import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    localStorage.removeItem('todo_token');

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render auth controls when unauthenticated', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Todo App');
    expect(compiled.textContent).toContain('Login');
    expect(compiled.textContent).toContain('Register');
  });

  it('should switch to register mode when register tab is clicked', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    const buttons = fixture.nativeElement.querySelectorAll('.auth-tabs button') as NodeListOf<HTMLButtonElement>;
    const registerButton = Array.from(buttons)
      .find((button) => button.textContent?.trim() === 'Register') as HTMLButtonElement;

    registerButton.click();
    fixture.detectChanges();

    expect(fixture.componentInstance.authMode).toBe('register');
    expect(fixture.nativeElement.textContent).toContain('Create account');
  });
});
