module Chapters

let toRelativePath directory extension fileName =
    sprintf "%s/%s.%s" directory fileName extension

/// this list impacts the order in which chapters will be merged
let chapters =
    [ "introduction"
      "functions"
      "type_system"
      "async"
      "web_dev_backend"
      "web_dev_frontend" ]
