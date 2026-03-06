describe('Todo App', () => {
  it('renders the todo page shell', () => {
    cy.visit('/');

    cy.contains('h1', 'Todo App').should('be.visible');
    cy.get('form.add-form').should('be.visible');
    cy.get('input[name="newTodo"]').should('be.visible');
    cy.contains('button', 'Add').should('be.visible');
  });

  it('allows typing a new todo title', () => {
    cy.visit('/');

    cy.get('input[name="newTodo"]').type('Buy milk');
    cy.get('input[name="newTodo"]').should('have.value', 'Buy milk');
  });

  it('shows the app subtitle', () => {
    cy.visit('/');

    cy.contains('ASP.NET API + Angular Frontend').should('be.visible');
  });
});
