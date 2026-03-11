describe('Todo App Core Flows', () => {
  const makeTitle = (label: string) => `${label}-${Date.now()}-${Math.floor(Math.random() * 10000)}`;
  const apiBase = 'http://localhost:5080/api/todos';

  it('loads app endpoint', () => {
    cy.request('http://localhost:4200').then((res) => {
      expect(res.status).to.eq(200);
      expect(res.body).to.contain('<app-root>');
    });
  });

  it('creates todo via API flow', () => {
    const title = makeTitle('create-api');

    cy.request('POST', apiBase, { title }).then((createRes) => {
      expect(createRes.status).to.eq(201);
      expect(createRes.body.title).to.eq(title);
      expect(createRes.body.isDone).to.eq(false);
    });
  });

  it('edits todo via API flow', () => {
    const source = makeTitle('edit-source');
    const target = makeTitle('edit-target');

    cy.request('POST', apiBase, { title: source }).then((createRes) => {
      expect(createRes.status).to.eq(201);
      const id = createRes.body.id;

      cy.request('PUT', `${apiBase}/${id}`, { title: target, isDone: false }).then((updateRes) => {
        expect(updateRes.status).to.eq(200);
        expect(updateRes.body.title).to.eq(target);
      });
    });
  });

  it('completes todo via API flow', () => {
    const title = makeTitle('complete');

    cy.request('POST', apiBase, { title }).then((createRes) => {
      expect(createRes.status).to.eq(201);
      const id = createRes.body.id;

      cy.request('PATCH', `${apiBase}/${id}/complete`, {}).then((completeRes) => {
        expect(completeRes.status).to.eq(200);
        expect(completeRes.body.isDone).to.eq(true);
      });
    });
  });

  it('deletes todo via API flow', () => {
    const title = makeTitle('delete');

    cy.request('POST', apiBase, { title }).then((createRes) => {
      expect(createRes.status).to.eq(201);
      const id = createRes.body.id;

      cy.request('DELETE', `${apiBase}/${id}`).then((deleteRes) => {
        expect(deleteRes.status).to.eq(204);
      });

      cy.request({ method: 'GET', url: `${apiBase}/${id}`, failOnStatusCode: false }).then((getRes) => {
        expect(getRes.status).to.eq(404);
      });
    });
  });
});
