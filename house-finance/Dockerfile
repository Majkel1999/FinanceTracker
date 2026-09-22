FROM node:22-bookworm-slim AS build

WORKDIR /app

COPY package.json package-lock.json ./
COPY api/package.json api/
COPY web/package.json web/

RUN npm ci

COPY api api
COPY web web

RUN npm run build

FROM node:22-bookworm-slim AS runtime

WORKDIR /app

ENV NODE_ENV=production
ENV PORT=3001
ENV DATABASE_PATH=/data/house-finance.db

COPY package.json package-lock.json ./
COPY api/package.json api/

RUN npm ci --workspace api --omit=dev

COPY --from=build /app/api/dist api/dist
COPY --from=build /app/web/dist web/dist

RUN mkdir -p /data

EXPOSE 3001

CMD ["npm", "run", "start", "-w", "api"]
