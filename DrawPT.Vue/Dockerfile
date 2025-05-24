# build
FROM node:lts-alpine AS build

# make the 'app' folder the current working directory
WORKDIR /app
COPY . .

# set env vars
ARG VITE_PUBLIC_API_URL
ARG VITE_PUBLIC_HUB_URL
ARG VITE_TENANT_ID
ARG VITE_CLIENT_ID
ENV VITE_PUBLIC_API_URL=$VITE_PUBLIC_API_URL
ENV VITE_PUBLIC_HUB_URL=$VITE_PUBLIC_HUB_URL
ENV VITE_TENANT_ID=$VITE_TENANT_ID
ENV VITE_CLIENT_ID=$VITE_CLIENT_ID

RUN npm i
RUN npm run build

# release
FROM mcr.microsoft.com/mirror/docker/library/nginx:1.21-alpine as release
RUN apk add --no-cache nodejs
WORKDIR /app
# get latest updates because 1.21-alpine is not updated yet
RUN apk upgrade && apk update
COPY --from=build /app/dist /usr/share/nginx/html
COPY --from=build /app/nginx/default.conf /etc/nginx/conf.d/default.conf
ENTRYPOINT ["nginx", "-g", "daemon off;"]
