module Chapters

let toRelativePath directory extension fileName =
    sprintf "%s/%s.%s" directory fileName extension

/// this list impacts the order in which chapters will be merged
let chapters =
    [ "introduction"
      "type_system"
      "async" ]