## Plan que cierra

PLAN-__ — <nombre del plan>. Responsable: @usuario

## Precondiciones verificadas

- [ ] El plan del que dependo está mergeado en `main`
- [ ] Solo toqué archivos que mi plan declara como propios

## Verificación ejecutada

Pegar la salida real, no describirla:

    dotnet build              -> 0 errores
    dotnet ef database update -> aplicada
    dotnet run                -> /swagger carga

## Checklist

- [ ] Compila sin errores
- [ ] Migraciones aplicadas si toqué modelos
- [ ] DTOs de entrada y salida, sin exponer entidades crudas
- [ ] Validaciones con Data Annotations
- [ ] Errores 400 y 404 manejados en los endpoints nuevos
- [ ] Sin claves ni secretos en el commit
- [ ] Actualicé la tabla de endpoints del README si agregué o cambié alguno
- [ ] Marqué mi plan como completado en `PLANES/README.md`

## Qué desbloquea este PR

PLAN-__ y PLAN-__ pueden empezar.
