# **Guía de Trabajo para Proyecto**

## **1. Organización del Proyecto**
El proyecto está dividido en cuatro capas principales:
1. **Business**: Contiene la lógica de negocio y servicios principales.
2. **Common**: Proporciona utilidades y modelos compartidos entre las capas.
3. **Data**: Maneja la interacción con las fuentes de datos (e.g., archivos DBF).
4. **Presentation**: Implementa la interfaz de usuario utilizando el patrón MVVM.

Cada capa tiene su propio archivo `README.md` que describe su propósito, estructura y ejemplos de uso.

---

## **2. Flujo de Trabajo para Nuevas Funcionalidades**
### **2.1 Crear una Rama de Desarrollo**
1. Cambia a la rama `main`:
   ```bash
   git checkout main
   ```
2. Asegúrate de que `main` esté actualizada:
   ```bash
   git pull origin main
   ```
3. Crea una nueva rama basada en `main`:
   ```bash
   git checkout -b feature/nueva-funcionalidad
   ```

### **2.2 Implementar los Cambios**
1. Identifica la capa afectada y realiza los cambios necesarios.
2. Sigue los principios de **SOLID**:
   - **SRP**: Cada clase debe tener una única responsabilidad.
   - **OCP**: Las clases deben ser abiertas para extensión, pero cerradas para modificación.
   - **LSP**: Las implementaciones pueden ser sustituidas por sus interfaces o clases base.
   - **ISP**: Las interfaces son específicas para sus consumidores.
   - **DIP**: Las dependencias son inyectadas y dependen de abstracciones.
3. Escribe pruebas (si aplica) y valida los cambios.

### **2.3 Realizar un Commit**
1. Agrega los cambios:
   ```bash
   git add .
   ```
2. Realiza un commit con un mensaje claro:
   ```bash
   git commit -m "feat: descripción de la nueva funcionalidad"
   ```

### **2.4 Crear un Pull Request (PR)**
1. Sube la rama al repositorio remoto:
   ```bash
   git push origin feature/nueva-funcionalidad
   ```
2. Crea un PR en GitHub para revisión:
   ```bash
   gh pr create --base main --head feature/nueva-funcionalidad --title "Nueva Funcionalidad" --body "Descripción del cambio"
   ```

### **2.5 Eliminar la Rama de Desarrollo**
Una vez que el PR ha sido aprobado y fusionado, elimina la rama de desarrollo para mantener el repositorio limpio.

1. Cambia a la rama `main` localmente:
   ```bash
   git checkout main
   ```

2. Actualiza la rama `main` localmente:
   ```bash
   git pull origin main
   ```

3. Verifica el estado del PR para asegurarte de que esté aprobado y fusionado:
   - Ver detalles del PR en la terminal:
     ```bash
     gh pr view <PR_NUMBER_OR_BRANCH>
     ```
   - Abrir el PR en el navegador:
     ```bash
     gh pr view <PR_NUMBER_OR_BRANCH> --web
     ```

4. Elimina la rama remota:
   ```bash
   git push origin --delete feature/nueva-funcionalidad
   ```

5. Elimina la rama local:
   ```bash
   git branch -d feature/nueva-funcionalidad
   ```
---

## **3. Flujo de Trabajo para Releases**
### **3.1 Crear una Rama de Release**
1. Cambia a la rama `main`:
   ```bash
   git checkout main
   ```
2. Asegúrate de que `main` esté actualizada:
   ```bash
   git pull origin main
   ```
3. Crea una nueva rama de release:
   ```bash
   git checkout -b release/vX.Y.Z
   ```

### **3.2 Preparar el Release**
1. Actualiza las versiones en los archivos `.csproj`.
2. Agrega los cambios al control de versiones:
   ```bash
   git add .
   ```
3. Realiza un commit con un mensaje claro:
   ```bash
   git commit -m "chore: actualiza versión a vX.Y.Z"
   ```
4. Sube los cambios al repositorio remoto:
   ```bash
   git push origin release/vX.Y.Z
   ```
5. Realiza pruebas finales en modo `Release`.

### **3.3 Crear un Tag**
1. Crea un tag para el release:
   ```bash
   git tag -a vX.Y.Z -m "Release vX.Y.Z - Descripción del release"
   git push origin vX.Y.Z
   ```

### **3.4 Fusionar en `main`**
1. Cambia a la rama `main`:
   ```bash
   git checkout main
   ```
2. Fusiona la rama de release:
   ```bash
   git merge release/vX.Y.Z --no-ff -m "Merge release/vX.Y.Z into main"
   git push origin main
   ```

### **3.5 Crear el Release en GitHub**
Una vez que la rama de release ha sido fusionada en `main`, puedes crear un release en GitHub utilizando la CLI:

1. Crear el release:
   ```bash
   gh release create vX.Y.Z --title "Release vX.Y.Z" --notes "Descripción detallada del release."
   ```

Reemplaza `vX.Y.Z` con el nombre del tag y proporciona una descripción adecuada para el release.

---

## **4. Buenas Prácticas**
1. **Mantén la Separación de Responsabilidades**:
   - Cada capa debe cumplir su propósito sin depender directamente de otras capas.
2. **Usa Inyección de Dependencias**:
   - Registra servicios en `ServiceCollectionExtensions.cs` para facilitar la configuración.
3. **Documenta Todo**:
   - Asegúrate de que cada capa tenga un `README.md` actualizado.
   - Usa el `CHANGELOG.md` para registrar cambios importantes.
4. **Pruebas y Validación**:
   - Realiza pruebas exhaustivas antes de fusionar cambios en `main`.

---

## **5. Herramientas Útiles**
- **`dotnet` CLI**:
  - Compilar en modo `Release`:
    ```bash
    dotnet build -c Release
    ```
  - Restaurar dependencias:
    ```bash
    dotnet restore
    ```

- **Git**:
  - Ver historial:
    ```bash
    git log --oneline --all --graph --decorate
    ```

- **GitHub CLI**:
  - Crear un PR:
    ```bash
    gh pr create --base main --head feature/nueva-funcionalidad --title "Nueva Funcionalidad" --body "Descripción del cambio"
    ```

---

## **6. Estándares de Commit**
Para mantener consistencia en los mensajes de commit, sigue el estándar de [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/):

1. **Formato del mensaje**:
   ```
   <tipo>(<área>): <descripción breve>
   
   [opcional] Cuerpo detallado
   [opcional] Footer
   ```

2. **Tipos comunes**:
   - `feat`: Nueva funcionalidad.
   - `fix`: Corrección de errores.
   - `chore`: Tareas de mantenimiento.
   - `docs`: Cambios en la documentación.
   - `style`: Cambios de formato o estilo.
   - `refactor`: Refactorización de código.
   - `test`: Adición o modificación de pruebas.

3. **Ejemplo**:
   ```
   feat(Business): Añade soporte para exportación a Excel
   
   Se implementa el servicio `ExcelExportService` para exportar datos a Excel con formato avanzado.
   ```

---

## **7. Revisión de Código**
### **7.1 Buenas Prácticas para Revisar PRs**
1. Verifica la calidad del código:
   - ¿Cumple con los principios SOLID?
   - ¿Es legible y está bien estructurado?
2. Revisa las pruebas:
   - ¿Hay pruebas unitarias suficientes?
   - ¿Las pruebas cubren casos extremos?
3. Asegúrate de que la documentación esté actualizada:
   - ¿Se actualizaron los archivos `README.md` y `CHANGELOG.md`?

---

## **8. Resolución de Conflictos**
### **8.1 Pasos para Resolver Conflictos**
1. Identifica los archivos en conflicto:
   ```bash
   git status
   ```
2. Abre los archivos y resuelve los conflictos manualmente.
3. Marca los conflictos como resueltos:
   ```bash
   git add <archivo>
   ```
4. Continúa con el merge:
   ```bash
   git merge --continue
   ```

---

## **9. Checklist para Releases**
### **9.1 Antes de Crear un Release**
1. Verifica que todas las pruebas pasen.
2. Asegúrate de que el `CHANGELOG.md` esté actualizado.
3. Revisa que la documentación esté completa.
4. Confirma que no hay conflictos pendientes.

---

## **10. Seguridad**
### **10.1 Manejo de Credenciales y Secretos**
1. Usa variables de entorno para credenciales sensibles.
2. Configura herramientas como Azure Key Vault para almacenar secretos.
3. Nunca incluyas credenciales en el código fuente.

---

## **11. Pruebas**
### **11.1 Tipos de Pruebas**
1. **Unitarias**: Validan funciones individuales.
2. **Integración**: Verifican la interacción entre componentes.
3. **Regresión**: Aseguran que cambios recientes no rompan funcionalidades existentes.

### **11.2 Ejecución de Pruebas**
1. Ejecuta pruebas unitarias:
   ```bash
   dotnet test
   ```
2. Revisa la cobertura:
   ```bash
   dotnet test --collect:"Code Coverage"
   ```

---

Esta guía estandariza el flujo de trabajo y asegura que todos los programadores sigan las mismas prácticas.
