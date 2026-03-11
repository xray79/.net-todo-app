describe('Todo App Auth Flow', () => {
  it('renders login/register controls by default', () => {
    cy.visit('/');

    cy.contains('button', 'Login').should('be.visible');
    cy.contains('button', 'Register').should('be.visible');
    cy.get('input[name="email"]').should('be.visible');
    cy.get('input[name="password"]').should('be.visible');
  });

  it('logs in and loads todos', () => {
    cy.intercept('POST', '**/api/auth/login', {
      statusCode: 200,
      body: { token: 'fake-jwt-token', email: 'user@example.com' }
    }).as('login');

    cy.intercept('GET', '**/api/todos', {
      statusCode: 200,
      body: [{ id: 1, title: 'Auth todo', isDone: false }]
    }).as('getTodos');

    cy.visit('/');
    cy.get('input[name="email"]').type('user@example.com');
    cy.get('input[name="password"]').type('password123');
    cy.get('.auth-form button[type="submit"]').click();

    cy.wait('@login');
    cy.wait('@getTodos');
    cy.window().then((window) => {
      expect(window.localStorage.getItem('todo_token')).to.eq('fake-jwt-token');
    });
  });

  it('switches to register and submits registration', () => {
    cy.intercept('POST', '**/api/auth/register', {
      statusCode: 200,
      body: { token: 'registered-token', email: 'new@example.com' }
    }).as('register');

    cy.intercept('GET', '**/api/todos', {
      statusCode: 200,
      body: []
    }).as('getTodos');

    cy.visit('/');
    cy.contains('button', 'Register').click();
    cy.get('input[name="email"]').type('new@example.com');
    cy.get('input[name="password"]').type('password123');
    cy.get('.auth-form button[type="submit"]').click();

    cy.wait('@register');
    cy.wait('@getTodos');
    cy.window().then((window) => {
      expect(window.localStorage.getItem('todo_token')).to.eq('registered-token');
    });
  });
});
